using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using System;
using Scenes.Ingame.Manager;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// PlayerStatus�̏������ƂɃQ�[����UI�Ƀv���C���[�S���̏���\��������ׂ̃X�N���v�g�i�f�[�^�Ǘ���S���j
    /// ��قǃl�b�g���[�N�ł́A�����l�̏��̕\���ɑΉ��ł���`��S������
    /// </summary>
    public class DisplayPlayerStatusManager : MonoBehaviour
    {

        public int playerNum { get { return _players.Length; } }//�Q�[���ɎQ�����Ă���v���C���[�̐l��
        [SerializeField] private GameObject[] _players;//�v���C���[��GameObject���i�[

        public PlayerStatus[] PlayerStatuses { get { return _playerStatuses; } }
        [SerializeField] private PlayerStatus[] _playerStatuses;

        private Subject<Unit> _completeSort = new Subject<Unit>();
        public IObservable<Unit> OnCompleteSort { get { return _completeSort; } }//_playerStatuses�z����쐬�E�\�[�g���I�����ۂɃC�x���g�����s 

        private void Start()
        {
            IngameManager.Instance.OnPlayerSpawnEvent
                .Subscribe(_ =>
                {
                    //�v���C���[�̐��𐔂���B�l�������Ƃɔz����쐬�B
                    //����͂����ɂ��鏈���S�Ă��uPlayer�̃X�|�[�������������v�C�x���g�����s���ꂽ��s���悤�ɂ���B(PlayerSpawner����Q���l�����擾�ł���悤�ɂ���)
                    _players = GameObject.FindGameObjectsWithTag("Player");
                    _playerStatuses = new PlayerStatus[_players.Length];

                    //PlayerID�̏����Ƀ\�[�g���s���B
                    //����ID��8���̐����ɂȂ邩���B���̎��͐��̑傫���ŏ����Ƀ\�[�g����悤�ɂ���
                    foreach (var player in _players)
                    {
                        int myID = player.GetComponent<PlayerStatus>().playerID;
                        _playerStatuses[myID] = player.GetComponent<PlayerStatus>();
                    }

                    //�z��̃\�[�g�I����ʒm����
                    _completeSort.OnNext(Unit.Default);
                    _completeSort.OnCompleted();
                }).AddTo(this);
        }
    }
}