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
        public bool endMove;
        private NavMeshAgent _myAgent;
        [SerializeField] EnemyStatus _enemyStatus;

        [SerializeField] private bool _staminaOver = false;



        private float _stuminaChangeCount = 0;//スタミナを毎秒減らすのに使用
       




        /// <summary>
        /// 初期化処理外部から呼び出す
        /// </summary>
        public void Init() {
            _myAgent = GetComponent<NavMeshAgent>();
            if (_myAgent == null) Debug.LogError("NavMeshAgentが認識できません");
            _myAgent.destination = this.transform.position;

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Vector3.Magnitude(this.transform.position - _myAgent.destination) < 1.5f) { endMove = true; } else { endMove = false; }

            _stuminaChangeCount += Time.deltaTime;
            if (_stuminaChangeCount > 1) 
            {//毎秒処理
                Debug.Log(_enemyStatus.Stamina);
                _stuminaChangeCount -= 1;
                switch (_enemyStatus.ReturnEnemyState)
                {
                    case EnemyState.Patorolling:
                        _myAgent.speed = _enemyStatus.ReturnPatolloringSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
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
                        else if(_enemyStatus.ReturnStaminaBase < _enemyStatus.Stamina)
                        {
                            _staminaOver = false;
                        }
                        break;
                    case EnemyState.Chese:
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
                            _myAgent.speed = _enemyStatus.ReturnSearchSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
                        }
                        else
                        {
                            _myAgent.speed = _enemyStatus.ReturnCheseSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
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
                            _myAgent.speed = _enemyStatus.ReturnSearchSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
                        }
                        else
                        {
                            _myAgent.speed = _enemyStatus.ReturnCheseSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
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

        public void SetMovePosition(Vector3 targetPosition) 
        {
            _myAgent.destination = targetPosition;
        }
    }
}