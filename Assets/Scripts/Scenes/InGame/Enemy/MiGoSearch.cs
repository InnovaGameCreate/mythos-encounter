using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class MiGoSearch : EnemySearch
    {
        [SerializeField]private bool _uniqueChase;//����ȃ`�F�C�X�����邩�ǂ���

        protected override void FixedUpdate()
        {
            if (_uniqueChase) {
                if (_myVisivilityMap != null)//���G�̏������ł��Ă��Ȃ��ꍇ
                {
                    Debug.LogError("�}�b�v��񂪂���܂���A_myVisivilityMap���쐬���Ă�������");
                    return;
                }
                if (_enemyStatus.ReturnEnemyState == EnemyState.Patrolling || _enemyStatus.ReturnEnemyState == EnemyState.Searching)
                { //�����Ԃ܂��͑{����Ԃ̏ꍇ
                    //����I�Ɏ��E���𒲂ׂ�
                    _checkTimeCount += Time.deltaTime;
                    if (_checkTimeCount > _checkRate)
                    {
                        _myEneyMove.SetMovePosition(_player.transform.position);
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
                            _enemyStatus.SetEnemyState(EnemyState.Chase);
                        }
                        else
                        {
                            if (_enemyStatus.ReturnReactToLight && _myVisivilityMap.RightCheck(this.transform.position, _player.transform.position, _visivilityRange, _playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&�͍�����]������鎖�ɒ���
                            { //���������邩���ׂ�
                                if (_debugMode) Debug.Log("����������");
                                _enemyStatus.SetEnemyState(EnemyState.Searching);
                            }
                            else if (Mathf.Pow((float)(valume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //�v���C���[�̑������������邩���ׂ�

                                _enemyStatus.SetEnemyState(EnemyState.Searching);
                                _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                            }
                            else
                            {
                                //�Ȃ�̍��Ղ�������Ȃ������ꍇ���ʂɏ��񂷂�
                                _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                                if (_myEneyMove.endMove)//�ړ����I����Ă���ꍇ
                                {
                                    //���Ղ̂������ꏊ�܂ŗ������������Ȃ������ꍇ���������s�����̂�Status������������
                                    _enemyStatus.SetEnemyState(EnemyState.Patrolling);
                                }
                            }
                        }
                    }
                }
            }
            else {
                base.FixedUpdate();
            }

        }

        public void ChangeUniqueChase(bool uniqueChase) { 
            _uniqueChase = uniqueChase;
        }

    }
}
