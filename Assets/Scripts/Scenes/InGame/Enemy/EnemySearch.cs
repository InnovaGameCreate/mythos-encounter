using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using static Scenes.Ingame.Enemy.EnemyVisibilityMap;
using Cysharp.Threading.Tasks.Triggers;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// �p�g���[������B�v���C���[�̍��Ղ�T���B�����Ԃƍ��G��Ԃ̓��������肵�A�ǐՂƍU����Ԃւ̈ڍs���s���B
    /// </summary>
    public class EnemySearch : MonoBehaviour
    {
        protected EnemyVisibilityMap _myVisivilityMap;
        [SerializeField]
        protected float _checkRate;//���b���ƂɎ��E�̏�Ԃ��`�F�b�N���邩
        protected float _checkTimeCount;//�O��`�F�b�N���Ă���̎��Ԃ��v��
        [SerializeField]
        protected bool _debugMode;
        [SerializeField]
        protected EnemyMove _myEneyMove;

        [SerializeField]
        protected float _visivilityRange;//�d�l�㎋�E�͈͂͑S�ē���H����Ȃ������炱���EnemyStatus�ɑ��荞�ނ�
        [SerializeField]
        protected EnemyStatus _enemyStatus;

        protected GameObject _player;
        protected PlayerStatus _playerStatus;
        protected float _audiomaterPower;
        protected Vector3 nextPositionCandidate = Vector3.zero;
        //���G�s���̃N���X�ł�
        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// �O�����炱�̃X�N���v�g�̏����ݒ�����邽�߂ɌĂяo��
        /// </summary>
        public void Init(EnemyVisibilityMap setVisivilityMap)
        {
            _myVisivilityMap = setVisivilityMap;
            _player = GameObject.Find("Player");
            if (_player == null) { Debug.LogWarning("�v���C���[���F���ł��܂���"); }
            _playerStatus = _player.GetComponent<PlayerStatus>();
            if (_playerStatus == null) { Debug.LogWarning("�v���C���[�X�e�[�^�X���F���ł��܂���"); }
            //�X�y�b�N�̏����ݒ�
            _audiomaterPower = _enemyStatus.ReturnAudiomaterPower;



            //�X�y�b�N�̕ύX���󂯎��
            _enemyStatus.OnAudiometerPowerChange.Subscribe(x => { _audiomaterPower = x; }).AddTo(this);


            _playerStatus.OnPlayerActionStateChange
                .Where(state => state == PlayerActionState.Walk || state == PlayerActionState.Dash || state == PlayerActionState.Sneak)
                .Subscribe(state =>
                {
                    //�v���C���[�̑����𕷂�
                    if (_enemyStatus.ReturnEnemyState == EnemyState.Patorolling || _enemyStatus.ReturnEnemyState == EnemyState.Searching)
                    {
                        float valume = 0;

                        switch (state)
                        {
                            case PlayerActionState.Sneak:
                                valume = _playerStatus.nowPlayerSneakVolume;
                                break;
                            case PlayerActionState.Walk:
                                valume = _playerStatus.nowPlayerWalkVolume;
                                break;
                            case PlayerActionState.Dash:
                                valume = _playerStatus.nowPlayerRunVolume;
                                break;
                        }

                        if (Mathf.Pow((float)(valume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                        {//�����������邩�ǂ���
                            _myVisivilityMap.HearingSound(_player.transform.position, 15, false);
                            _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                        }
                    }
                }).AddTo(this);

        }


        protected virtual void FixedUpdate()
        {
            if (_myVisivilityMap != null)//���G�̏������ł��Ă��Ȃ��ꍇ
            {
                Debug.LogError("�}�b�v��񂪂���܂���A_myVisivilityMap���쐬���Ă�������");
                return;
            }

            if (_enemyStatus.ReturnEnemyState == EnemyState.Patorolling || _enemyStatus.ReturnEnemyState == EnemyState.Searching)
            { //�����Ԃ܂��͑{����Ԃ̏ꍇ



                //����I�Ɏ��E���𒲂ׂ�
                _checkTimeCount += Time.deltaTime;
                if (_checkTimeCount > _checkRate)
                {
                    float valume = 0;//�v���C���[�̑������L�^
                    switch (_playerStatus.nowPlayerActionState)
                    {
                        case PlayerActionState.Sneak:
                            valume = _playerStatus.nowPlayerSneakVolume;
                            if (_debugMode) Debug.Log("�E�ԉ�����������");
                            break;
                        case PlayerActionState.Walk:
                            valume = _playerStatus.nowPlayerWalkVolume;
                            if (_debugMode) Debug.Log("����������������");
                            break;
                        case PlayerActionState.Dash:
                            valume = _playerStatus.nowPlayerRunVolume;
                            if (_debugMode) Debug.Log("���鉹����������");
                            break;
                    }
                    _checkTimeCount = 0;
                    //�����Ȃ��̂𒲂ׂ�B����͌���I�Ȃ��̂قǗD�悵�ĔF������
                    if (CheckCanPlayerVisivlleCheck())
                    { //�v���C���[�̎p�������邩���ׂ�
                      _myEneyMove.SetMovePosition(_player.transform.position);
                      _enemyStatus.SetEnemyState(EnemyState.Chese);
                    }
                    
                    else if (_enemyStatus.ReturnReactToLight&& _myVisivilityMap.RightCheck(this.transform.position,_player.transform.position,_visivilityRange,_playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&�͍�����]������鎖�ɒ���
                    { //���������邩���ׂ�
                        if (_debugMode) Debug.Log("����������");
                        _enemyStatus.SetEnemyState(EnemyState.Searching);
                        _myEneyMove.SetMovePosition(nextPositionCandidate);
                    }
                    
                    else if (Mathf.Pow((float)(valume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0) 
                    { //�v���C���[�̑������������邩���ׂ�
 
                        _enemyStatus.SetEnemyState(EnemyState.Searching);
                        _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                        _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                    }
                    
                    else
                    {
                        //�Ȃ�̍��Ղ�������Ȃ������ꍇ���ʂɏ��񂷂�
                        _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                        
                        if (_myEneyMove.endMove)//�ړ����I����Ă���ꍇ
                        {
                            
                            //���Ղ̂������ꏊ�܂ŗ������������Ȃ������ꍇ���������s�����̂�Status������������
                            _enemyStatus.SetEnemyState(EnemyState.Patorolling);
                            //���炽�Ȉړ�����擾���郁�\�b�h����������
                            _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            
                        }
                            
                        
                    }
                    
                    
                    
                }
            }
        }

        protected bool CheckCanPlayerVisivlleCheck() {
            float range = Vector3.Magnitude(this.transform.position - _player.transform.position);//�����������߂�̂͂������R�X�g���d���炵���̂Ŋm���Ɍv�Z���K�v�ɂȂ��Ă��炵�Ă܂�
                                             //���E���ʂ邩��Ray���ʂ邩
            bool hit;
            Ray ray = new Ray(this.transform.position, _player.transform.position - this.transform.position);
            hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, -1, QueryTriggerInteraction.Collide);
            if (!hit)
            { //���ɂ��������Ă��Ȃ������ꍇ
                if (_debugMode) { Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 3); Debug.Log("�v���C���[����"); }
                return true;
            }
            else { 
                return false;
            }
        }


    }
}
