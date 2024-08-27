using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using Scenes.Ingame.Player;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵キャラクターの移動を管理する
    /// </summary>
    public class EnemyMove : MonoBehaviour
    {
        public bool _endMove;
        private NavMeshAgent _myAgent;
        [SerializeField] EnemyStatus _enemyStatus;

        [SerializeField] private bool _staminaOver = false;
        /// <summary> 硬直しているか否か</summary>
        private bool _stiffness;

        private float _staminaChangeCount = 0;//スタミナを毎秒減らすのに使用
        private Vector3 _movePosition;

        public Vector3 GetMovePosition() {
            return _movePosition;
        }

        private Vector3 _initialPosition = new Vector3(30, 0, 18);//初期位置保存用変数






        /// <summary>
        /// 初期化処理外部から呼び出す
        /// </summary>
        public void Init() {
            _myAgent = GetComponent<NavMeshAgent>();
            if (_myAgent == null) Debug.LogError("NavMeshAgentが認識できません");
            _myAgent.destination = this.transform.position;
            _initialPosition = this.transform.position;
            _enemyStatus.OnStiffnessTimeChange.Subscribe(stiffnessTime => {
                if (stiffnessTime > 0)
                {
                    _stiffness = true;
                }
                else {
                    _stiffness = false;
                }
            }
            ).AddTo(this);

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Vector3.Magnitude(this.transform.position - _myAgent.path.corners[_myAgent.path.corners.Length - 1]) < 1.5f)
            {
                _endMove = true;
            } else
            {
                _endMove = false;
            }

            _staminaChangeCount += Time.deltaTime;
            if (_staminaChangeCount > 1)
            {//毎秒処理
                _staminaChangeCount -= 1;

                if (_stiffness)
                {//硬直中は移動不能に
                    _myAgent.speed = 0;
                }
                else
                {

                    switch (_enemyStatus.ReturnEnemyState)
                    {
                        case EnemyState.Patrolling:
                            _myAgent.speed = _enemyStatus.ReturnPatrollingSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1) * (_enemyStatus.ReturnWaterEffectDebuff ? 0.8f : 1);
                            if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                            { //スタミナが削れていたら
                                _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                            }
                            else if (_enemyStatus.ReturnStaminaBase < _enemyStatus.Stamina)
                            {


                                _staminaOver = false;
                            }
                            break;
                        case EnemyState.Searching:
                            _myAgent.speed = _enemyStatus.ReturnSearchSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
                            if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                            { //スタミナが削れていたら
                                _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                            }
                            else if (_enemyStatus.ReturnStaminaBase < _enemyStatus.Stamina)
                            {
                                _staminaOver = false;
                            }
                            break;
                        case EnemyState.Chase:
                            //スタミナ周りの処理をする
                            if (_staminaOver)
                            { //スタミナが切れ切ったかどうか
                                if (_enemyStatus.ReturnStaminaBase <= _enemyStatus.Stamina && !(_enemyStatus.ReturnStaminaBase == 0))
                                { //回復した状態にあるかどうか
                                    _staminaOver = false;
                                }
                                else
                                {//回復しきっていないなら回復する
                                    if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                                    { //スタミナが削れていたら、これがあるのはスタミナが0のキャラがいた時にまともに動かすため
                                        _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                    }
                                }

                            }
                            else
                            { //まだスタミナが切れ切って無い場合
                                if (0 >= _enemyStatus.Stamina)
                                { //たった今切れ切ったかどうか
                                    _staminaOver = true;
                                    if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                                    { //スタミナが削れていたら、これがあるのはスタミナが0のキャラがいた時にまともに動かすため
                                        _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                    }
                                }
                                else
                                {
                                    if (0 < _enemyStatus.Stamina)
                                    { //スタミナを削れるなら、これがあるのはスタミナが0のキャラがいた時にまともに動かすため
                                        _enemyStatus.StaminaChange(_enemyStatus.Stamina - 1);
                                    }
                                }
                            }




                            if (_staminaOver)
                            {
                                _myAgent.speed = _enemyStatus.ReturnSearchSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1) * (_enemyStatus.ReturnWaterEffectDebuff ? 0.8f : 1);
                            }
                            else
                            {
                                _myAgent.speed = _enemyStatus.ReturnChaseSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1) * (_enemyStatus.ReturnWaterEffectDebuff ? 0.8f : 1);
                            }
                            break;


                        case EnemyState.Attack:
                            //スタミナ周りの処理をする
                            if (_staminaOver)
                            { //スタミナが切れ切ったかどうか
                                if (_enemyStatus.ReturnStaminaBase <= _enemyStatus.Stamina && !(_enemyStatus.ReturnStaminaBase == 0))
                                { //回復した状態にあるかどうか
                                    _staminaOver = false;
                                }
                                else
                                {//回復しきっていないなら回復する
                                    if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                                    { //スタミナが削れていたら、これがあるのはスタミナが0のキャラがいた時にまともに動かすため
                                        _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                    }
                                }
                            }
                            else
                            { //まだスタミナが切れ切って無い場合
                                if (0 >= _enemyStatus.Stamina)
                                { //たった今切れ切ったかどうか
                                    _staminaOver = true;
                                    if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                                    { //スタミナが削れていたら、これがあるのはスタミナが0のキャラがいた時にまともに動かすため
                                        _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                    }
                                }
                                else
                                {
                                    if (0 < _enemyStatus.Stamina)
                                    { //スタミナを削れるなら、これがあるのはスタミナが0のキャラがいた時にまともに動かすため
                                        _enemyStatus.StaminaChange(_enemyStatus.Stamina - 1);
                                    }
                                }
                            }





                            if (_staminaOver)
                            {
                                _myAgent.speed = _enemyStatus.ReturnSearchSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1) * (_enemyStatus.ReturnWaterEffectDebuff ? 0.8f : 1);
                            }
                            else
                            {
                                _myAgent.speed = _enemyStatus.ReturnChaseSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1) * (_enemyStatus.ReturnWaterEffectDebuff ? 0.8f : 1);
                            }
                            break;
                        case EnemyState.FallBack:

                            break;
                        default:
                            Debug.LogWarning("想定外のEnemyStatus");
                            break;


                    }
                }
            }

           
        }

        public void SetMovePosition(Vector3 targetPosition)
        {
            _movePosition = targetPosition;
            _myAgent.destination = targetPosition;
        }

        /// <summary>
        /// 座標を初期位置に移動する関数
        /// </summary>
        public void ResetPosition()
        {
            _myAgent.enabled = false;
            _enemyStatus.SetEnemyState(EnemyState.Patrolling);
            transform.position = _initialPosition;
            _endMove = true;
            _myAgent.enabled = true;

        }

        /// <summary>
        /// 硬直状態を変更します
        /// </summary>
        /// <param name="value">変更する値</param>
        public void ChangeStiffness(bool value)
        {
            _stiffness = value;
        }
    } 
}
