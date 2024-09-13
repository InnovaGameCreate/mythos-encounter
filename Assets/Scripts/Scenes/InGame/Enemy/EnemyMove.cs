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

        /// <summary> 硬直しているか否か</summary>
        private bool _stiffness = false;

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
                SpeedChange();
            }
            ).AddTo(this);

           _enemyStatus.OnBindChange
                .Skip(1)//初期化の時は無視
                .Subscribe(x =>
                {
                    SpeedChange();
                }).AddTo(this);
            _enemyStatus.OnEnemyStateChange.Subscribe(x =>{
                SpeedChange();
            }).AddTo(this);
            _enemyStatus.OnIsWaterEffectDebuffChange.Skip(1).Subscribe(x =>
            { 
                SpeedChange(); 
            }).AddTo(this);
            _enemyStatus.OnSpeedChange.Subscribe(x => 
            {
                _myAgent.speed = x;
            }).AddTo(this);
            _enemyStatus.OnStaminaOverChange.Subscribe(x => 
            { 
              SpeedChange();
            }).AddTo(this);
            SpeedChange();
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

                switch (_enemyStatus.EnemyState)
                {
                    case EnemyState.Patrolling:
                    case EnemyState.Searching:
                        //通常の場合
                        if (_enemyStatus.StaminaBase > _enemyStatus.Stamina)
                        { //スタミナが削れていたら
                            _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                        }
                        else if (_enemyStatus.StaminaBase < _enemyStatus.Stamina)
                        {
                            _enemyStatus.SetStuminaOver(false);
                        }
                        break;
                    case EnemyState.Chase:
                    case EnemyState.Attack:
                        //走る場合
                        if (_enemyStatus.StaminaOver)
                        { //スタミナが切れ切ったかどうか
                            if (_enemyStatus.StaminaBase <= _enemyStatus.Stamina && !(_enemyStatus.StaminaBase == 0))
                            { //回復した状態にあるかどうか
                                _enemyStatus.SetStuminaOver(false);
                            }
                            else
                            {//回復しきっていないなら回復する
                                if (_enemyStatus.StaminaBase > _enemyStatus.Stamina)
                                { //スタミナが削れていたら、これがあるのはスタミナが0のキャラがいた時にまともに動かすため
                                    _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                }
                            }
                        }
                        else
                        { //まだスタミナが切れ切って無い場合
                            if (0 >= _enemyStatus.Stamina)
                            { //たった今切れ切ったかどうか
                                _enemyStatus.SetStuminaOver(true);
                                if (_enemyStatus.StaminaBase > _enemyStatus.Stamina)
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
                        break;
                    case EnemyState.FallBack:

                        break;
                        case EnemyState.Discover: 
                        break;
                    default:
                        Debug.LogWarning("想定外のEnemyStatus");
                        break;

                }


                
            }

        }

            private void SpeedChange() {
            

                if (_stiffness)
                {//硬直中は移動不能に
                    _myAgent.speed = 0;
                }
                else
                {
                    switch (_enemyStatus.EnemyState)
                    {
                        case EnemyState.Patrolling:
                            _enemyStatus.SetSpeed(_enemyStatus.PatrollingSpeed * (_enemyStatus.IsBind ? 0.1f : 1) * (_enemyStatus.WaterEffectDebuff ? 0.8f : 1));

                            break;
                        case EnemyState.Searching:
                            _enemyStatus.SetSpeed(_enemyStatus.SearchSpeed * (_enemyStatus.IsBind ? 0.1f : 1) * (_enemyStatus.WaterEffectDebuff ? 0.8f : 1));

                            break;
                        case EnemyState.Chase:
                            if (_enemyStatus.StaminaOver)
                            {
                                _enemyStatus.SetSpeed(_enemyStatus.SearchSpeed * (_enemyStatus.IsBind ? 0.1f : 1) * (_enemyStatus.WaterEffectDebuff ? 0.8f : 1));
                            }
                            else
                            {
                                _enemyStatus.SetSpeed(_enemyStatus.ChaseSpeed * (_enemyStatus.IsBind ? 0.1f : 1) * (_enemyStatus.WaterEffectDebuff ? 0.8f : 1));
                            }
                            break;


                        case EnemyState.Attack:
                            
                            if (_enemyStatus.StaminaOver)
                            {
                                _enemyStatus.SetSpeed(_enemyStatus.SearchSpeed * (_enemyStatus.IsBind ? 0.1f : 1) * (_enemyStatus.WaterEffectDebuff ? 0.8f : 1));
                            }
                            else
                            {
                                _enemyStatus.SetSpeed(_enemyStatus.ChaseSpeed * (_enemyStatus.IsBind ? 0.1f : 1) * (_enemyStatus.WaterEffectDebuff ? 0.8f : 1));
                            }
                            break;
                        case EnemyState.FallBack:

                            break;
                    case EnemyState.Discover:
                        _enemyStatus.SetSpeed(0);
                        break;
                        default:
                            Debug.LogWarning("想定外のEnemyStatus");
                            break;


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

    } 
}
