using Fusion;
using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class MiGoSearch : EnemySearch
    {
        [Networked] private bool _uniqueChase { get; set; }//����ȃ`�F�C�X�����邩�ǂ���

        public override void FixedUpdateNetwork()
        {
            if (_uniqueChase) {
                if (_myVisivilityMap != null)//���G�̏������ł��Ă��Ȃ��ꍇ
                {
                    Debug.LogError("�}�b�v��񂪂���܂���A_myVisivilityMap���쐬���Ă�������");
                    return;
                }
                if (_enemyStatus.State == EnemyState.Patrolling || _enemyStatus.State == EnemyState.Searching)
                { //�����Ԃ܂��͑{����Ԃ̏ꍇ
                    //����I�Ɏ��E���𒲂ׂ�
                    _checkTimeCount += Runner.DeltaTime;
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
                        if (CheckCanSeeThePlayer())
                        { //�v���C���[�̎p�������邩���ׂ�
                            _myEneyMove.SetMovePosition(_player.transform.position);
                            _enemyStatus.SetEnemyState(EnemyState.Chase);
                        }
                        else
                        {
                            if (_enemyStatus.ReactToLight && _myVisivilityMap.RightCheck(this.transform.position, _player.transform.position, _visivilityRange, _playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&�͍�����]������鎖�ɒ���
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
                                if (_myEneyMove._endMove)//�ړ����I����Ă���ꍇ
                                {
                                    _myVisivilityMap.ChangeGridWatchNum(_myEneyMove.GetMovePosition(), 1, true);
                                    //���Ղ̂������ꏊ�܂ŗ������������Ȃ������ꍇ���������s�����̂�Status������������
                                    _enemyStatus.SetEnemyState(EnemyState.Patrolling);
                                }
                            }
                        }
                    }
                }
            }
            else {
                base.FixedUpdateNetwork();
            }

        }

        public void ChangeUniqueChase(bool uniqueChase) { 
            _uniqueChase = uniqueChase;
        }

    }
}
