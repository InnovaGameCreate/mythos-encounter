using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using System;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// PlayerStatus�̏������ƂɃQ�[����UI�Ƀv���C���[�S���̏���\��������ׂ̃X�N���v�g
    /// ��قǃl�b�g���[�N�ł́A�����l�̏��̕\���ɑΉ��ł���`��S������
    /// MV(R)P�ɂ�����View�̖�����z��
    /// </summary>
    public class DisplayPlayerStatusManager : MonoBehaviour
    {
        [SerializeField] private Slider[] _healthSliders;
        [SerializeField] private Slider[] _sanValueSliders;
        [SerializeField] private TMP_Text[] _healthText;
        [SerializeField] private TMP_Text[] _sanValueText;

        public int playerNum { get { return _players.Length; } }//�Q�[���ɎQ�����Ă���v���C���[�̐l��
        [SerializeField] private GameObject[] _players;//�v���C���[��GameObject���i�[

        public PlayerStatus[] PlayerStatuses { get { return _playerStatuses; } }
        [SerializeField] private PlayerStatus[] _playerStatuses;

        private Subject<Unit> _completeSort = new Subject<Unit>();
        public IObservable<Unit> OnCompleteSort { get { return _completeSort; } }//_playerStatuses�z����쐬�E�\�[�g���I�����ۂɃC�x���g�����s 

        private void Start()
        {
            //�v���C���[�̐��𐔂���B�l�������Ƃɔz����쐬�B
            _players = GameObject.FindGameObjectsWithTag("Player");
            _playerStatuses = new PlayerStatus[_players.Length];

            //PlayerID�̏����Ƀ\�[�g���s���B
            foreach (var player in _players)
            {
                int myID = player.GetComponent<PlayerStatus>().playerID;
                _playerStatuses[myID] = player.GetComponent<PlayerStatus>();

                //�z��̃\�[�g�I����ʒm����
                _completeSort.OnNext(Unit.Default);
                _completeSort.OnCompleted();
            }
        }
        /// <summary>
        /// Slider�̒l��ς���ׂ̊֐�
        /// </summary>
        /// <param name="value">Slinder.Value�ɑ������l</param>
        /// <param name="ID">�v���C���[ID</param>
        /// <param name="mode">Health(�̗�), SanValue(SAN�l)�ǂ����ύX����̂�������</param>
        public void ChangeSliderValue(int value , int ID, string mode)
        {
            Debug.Log(ID);
            if (mode == "Health")
            {
                _healthSliders[ID].value = value;
                _healthText[ID].text = value.ToString();
            }

            else if (mode == "SanValue")
            { 
                _sanValueSliders[ID].value = value;
                _sanValueText[ID].text = value.ToString();
            }
                
        }
    }
}

