using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// PlayerStatus��DisplayPlayerStatusManager�̋��n�����s���N���X
    /// MV(R)P�ɂ�����Presenter�̖�����z��
    /// </summary>
    public class PlayerGUIPresenter : MonoBehaviour
    {
        //model
        [SerializeField] private PlayerStatus[] _playerStatuses;
        //View
        [SerializeField] private DisplayPlayerStatusManager _displayPlayerStatusManager;

        //��ʓ���UI�̗�
        [SerializeField] private Slider[] _healthSliders;
        [SerializeField] private Slider[] _sanValueSliders;
        [SerializeField] private TMP_Text[] _healthText;
        [SerializeField] private TMP_Text[] _sanValueText;

        // Start is called before the first frame update
        void Awake()
        {
            _displayPlayerStatusManager.OnCompleteSort
                .FirstOrDefault()
                .Subscribe(_ => 
                {
                    //DisplayPlayerStatusManager�ɂ���\�[�g�ς݂̔z����擾
                    _playerStatuses = _displayPlayerStatusManager.PlayerStatuses;

                    //�e�v���C���[��HP��SAN�l���ύX���ꂽ�Ƃ��̏�����ǉ�����B
                    foreach (var playerStatus in _playerStatuses)
                    {
                        playerStatus.OnPlayerHealthChange
                            .Subscribe(x =>
                            {
                                //view�ɔ��f
                                ChangeSliderValue(x, playerStatus.playerID, "Health");
                            }).AddTo(this);

                        playerStatus.OnPlayerSanValueChange
                            .Subscribe(x =>
                            {
                                //view�ɔ��f
                                ChangeSliderValue(x, playerStatus.playerID, "SanValue");
                            }).AddTo(this);
                    }
                }).AddTo(this);
        }

        /// <summary>
        /// Slider�̒l��ς���ׂ̊֐�.Slider,Text�ɒ��ڎQ�Ƃ��Ă���
        /// </summary>
        /// <param name="value">Slinder.Value�ɑ������l</param>
        /// <param name="ID">�v���C���[ID</param>
        /// <param name="mode">Health(�̗�), SanValue(SAN�l)�ǂ����ύX����̂�������</param>
        public void ChangeSliderValue(int value, int ID, string mode)
        {
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
