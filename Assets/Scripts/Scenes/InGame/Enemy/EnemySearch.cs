using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scenes.Ingame.Player;
using UniRx;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// �p�g���[������B�v���C���[�̍��Ղ�T���B�����Ԃƍ��G��Ԃ̓��������肵�A�ǐՂƍU����Ԃւ̈ڍs���s���B
    /// </summary>
    public class EnemySearch : MonoBehaviour
    {
        private EnemyVisibilityMap _myVisivilityMap;
        [SerializeField]
        private float _checkRate;//���b���ƂɎ��E�̏�Ԃ��`�F�b�N���邩
        private float _checkTimeCount;//�O��`�F�b�N���Ă���̎��Ԃ��v��
        [SerializeField]
        private bool _debugMode;
        [SerializeField]
        private EnemyMove _myEneyMove;

        [SerializeField]
        private float _visivilityRange;//�d�l�㎋�E�͈͂͑S�ē���H����Ȃ������炱���EnemyStatus�ɑ��荞�ނ�
        [SerializeField]
        private EnemyStatus _enemyStatus;

        private GameObject _player;
        private PlayerStatus _playerStatus;
        private float _audiomaterPower;

        //���G�s���̃N���X�ł�
        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// �O�����炱�̃X�N���v�g�̏����ݒ�����邽�߂ɌĂяo��
        /// </summary>
        public void Init(){
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
                        if (_playerStatus.nowPlayerActionState == PlayerActionState.Sneak)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerSneakVolume * _audiomaterPower * 0.01f),2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x,2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2)))  > 0) 
                            { //�����������邩�ǂ���
                                _myVisivilityMap.HearingSound(_player.transform.position,15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                        else if(_playerStatus.nowPlayerActionState == PlayerActionState.Walk)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerWalkVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //�����������邩�ǂ���
                                _myVisivilityMap.HearingSound(_player.transform.position, 15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        } else if (_playerStatus.nowPlayerActionState == PlayerActionState.Dash)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerRunVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //�����������邩�ǂ���
                                _myVisivilityMap.HearingSound(_player.transform.position, 15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
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
                    _checkTimeCount = 0;
                    //�����Ȃ��̂𒲂ׂ�B����͌���I�Ȃ��̂قǗD�悵�ĔF������
                    if (false)
                    { //�v���C���[�̎p�������邩���ׂ�

                    }
                    else if (false)
                    { //���������邩���ׂ�

                    }
                    else if (_enemyStatus.ReturnEnemyState == EnemyState.Patorolling || _enemyStatus.ReturnEnemyState == EnemyState.Searching)//�v���C���[�����𗧂ĂĂ��邩�ǂ���
                    {
                        if (_playerStatus.nowPlayerActionState == PlayerActionState.Sneak)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerSneakVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //�����������邩�ǂ���
                                if (_debugMode) Debug.Log("�E�ԉ�����������");
                                _myVisivilityMap.HearingSound(_player.transform.position,  15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                        else if (_playerStatus.nowPlayerActionState == PlayerActionState.Walk)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerWalkVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //�����������邩�ǂ���
                                if (_debugMode) Debug.Log("����������������");
                                _myVisivilityMap.HearingSound(_player.transform.position,  15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                        else if (_playerStatus.nowPlayerActionState == PlayerActionState.Dash)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerRunVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //�����������邩�ǂ���
                                if (_debugMode) Debug.Log("���鉹����������");
                                _myVisivilityMap.HearingSound(_player.transform.position, 15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                        else if (_debugMode) Debug.Log("���͕������Ȃ�");
                    }
                    else {
                        //�Ȃ�̍��Ղ�������Ȃ������ꍇ���ʂɏ��񂷂�
                        _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                        if (_myEneyMove.endMove)//�ړ����I����Ă���ꍇ
                        { 
                            //���Ղ̂������ꏊ�܂ŗ������������Ȃ������ꍇ���������s�����̂�Status������������
                            _enemyStatus.ChangeEnemyState(EnemyState.Patorolling);
                            //���炽�Ȉړ�����擾���郁�\�b�h����������
                            _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                        }
                    }
                }


            }
        }


        public void SetVisivilityMap(EnemyVisibilityMap setVisivilityMap) 
        { 
            _myVisivilityMap = setVisivilityMap;
        }


    }
}
