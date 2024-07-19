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
    /// �G�L�����N�^�[�̈ړ����Ǘ�����
    /// </summary>
    public class EnemyMove : MonoBehaviour
    {
        public bool endMove;
        private NavMeshAgent _myAgent;
        [SerializeField] EnemyStatus _enemyStatus;

        [SerializeField] private bool _staminaOver = false;


        private float _staminaChangeCount = 0;//�X�^�~�i�𖈕b���炷�̂Ɏg�p
        private Vector3 _initialPosition = new Vector3(30,0,18);//�����ʒu�ۑ��p�ϐ�
       




        /// <summary>
        /// �����������O������Ăяo��
        /// </summary>
        public void Init() {
            _myAgent = GetComponent<NavMeshAgent>();
            if (_myAgent == null) Debug.LogError("NavMeshAgent���F���ł��܂���");
            _myAgent.destination = this.transform.position;
            _initialPosition = this.transform.position;

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Vector3.Magnitude(this.transform.position - _myAgent.destination) < 1.5f) { endMove = true; } else { endMove = false; }

            _staminaChangeCount += Time.deltaTime;
            if (_staminaChangeCount > 1) 
            {//���b����
                Debug.Log(_enemyStatus.Stamina);
                _staminaChangeCount -= 1;
                switch (_enemyStatus.ReturnEnemyState)
                {
                    case EnemyState.Patrolling:
                        _myAgent.speed = _enemyStatus.ReturnPatrollingSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
                        if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                        { //�X�^�~�i�����Ă�����
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
                        { //�X�^�~�i�����Ă�����
                            _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                        }
                        else if(_enemyStatus.ReturnStaminaBase < _enemyStatus.Stamina)
                        {
                            _staminaOver = false;
                        }
                        break;
                    case EnemyState.Chase:
                        //�X�^�~�i����̏���������
                        if (_staminaOver)
                        { //�X�^�~�i���؂�؂������ǂ���
                            if (_enemyStatus.ReturnStaminaBase <= _enemyStatus.Stamina && !(_enemyStatus.ReturnStaminaBase == 0))
                            { //�񕜂�����Ԃɂ��邩�ǂ���
                                _staminaOver = false;
                            }
                            else
                            {//�񕜂������Ă��Ȃ��Ȃ�񕜂���
                                if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                                { //�X�^�~�i�����Ă�����A���ꂪ����̂̓X�^�~�i��0�̃L�������������ɂ܂Ƃ��ɓ���������
                                    _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                }
                            }

                        }
                        else
                        { //�܂��X�^�~�i���؂�؂��Ė����ꍇ
                            if (0 >= _enemyStatus.Stamina)
                            { //���������؂�؂������ǂ���
                                _staminaOver = true;
                                if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                                { //�X�^�~�i�����Ă�����A���ꂪ����̂̓X�^�~�i��0�̃L�������������ɂ܂Ƃ��ɓ���������
                                    _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                }
                            }
                            else
                            {
                                if (0 < _enemyStatus.Stamina)
                                { //�X�^�~�i������Ȃ�A���ꂪ����̂̓X�^�~�i��0�̃L�������������ɂ܂Ƃ��ɓ���������
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
                            _myAgent.speed = _enemyStatus.ReturnChaseSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
                        }
                        break;
                    case EnemyState.Attack:
                        //�X�^�~�i����̏���������
                        if (_staminaOver)
                        { //�X�^�~�i���؂�؂������ǂ���
                            if (_enemyStatus.ReturnStaminaBase <= _enemyStatus.Stamina && !(_enemyStatus.ReturnStaminaBase == 0))
                            { //�񕜂�����Ԃɂ��邩�ǂ���
                                _staminaOver = false;
                            }
                            else
                            {//�񕜂������Ă��Ȃ��Ȃ�񕜂���
                                if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                                { //�X�^�~�i�����Ă�����A���ꂪ����̂̓X�^�~�i��0�̃L�������������ɂ܂Ƃ��ɓ���������
                                    _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                }
                            }
                        }
                        else
                        { //�܂��X�^�~�i���؂�؂��Ė����ꍇ
                            if (0 >= _enemyStatus.Stamina)
                            { //���������؂�؂������ǂ���
                                _staminaOver = true;
                                if (_enemyStatus.ReturnStaminaBase > _enemyStatus.Stamina)
                                { //�X�^�~�i�����Ă�����A���ꂪ����̂̓X�^�~�i��0�̃L�������������ɂ܂Ƃ��ɓ���������
                                    _enemyStatus.StaminaChange(_enemyStatus.Stamina + 1);
                                }
                            }
                            else
                            {
                                if (0 < _enemyStatus.Stamina)
                                { //�X�^�~�i������Ȃ�A���ꂪ����̂̓X�^�~�i��0�̃L�������������ɂ܂Ƃ��ɓ���������
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
                            _myAgent.speed = _enemyStatus.ReturnChaseSpeed * (_enemyStatus.ReturnBind ? 0.1f : 1);
                        }
                        break;
                    case EnemyState.FallBack: 

                        break;
                    default:
                        Debug.LogWarning("�z��O��EnemyStatus");
                        break;
                    
                }
            }
        }

        public void SetMovePosition(Vector3 targetPosition) 
        {
            _myAgent.destination = targetPosition;
        }

        /// <summary>
        /// ���W�������ʒu�Ɉړ�����֐�
        /// </summary>
        public void ResetPosition()
        {
            _myAgent.enabled = false;
            _enemyStatus.SetEnemyState(EnemyState.Patrolling);
            transform.position = _initialPosition;
            _myAgent.enabled = true;

        }
    }
}