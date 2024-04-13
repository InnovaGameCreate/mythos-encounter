using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

        [Header("�Q�[����UI(�I�����C��)")]
        [SerializeField] private Slider[] _healthSliders;//�e�v���C���[��HP�o�[
        [SerializeField] private Slider[] _sanValueSliders;//�e�v���C���[��SAN�l�o�[
        [SerializeField] private TMP_Text[] _healthText;//�e�v���C���[��HP�c�ʕ\���e�L�X�g
        [SerializeField] private TMP_Text[] _sanValueText;//�e�v���C���[��SAN�l�c�ʕ\���e�L�X�g

        [Header("�Q�[����UI(�I�t���C��)")]
        [SerializeField] private Image _staminaGaugeBackGround;//�l�̃X�^�~�i�Q�[�W
        [SerializeField] private RectTransform _staminaGaugeFrontRect;//�l�̃X�^�~�i�Q�[�W
        [SerializeField] private Image _staminaGaugeFrontImage;//�l�̃X�^�~�i�Q�[�W

        private int _myPlayerID = 0;

        //�X�^�~�i�Q�[�W�֘A�̃t�B�[���h
        private float _defaulStaminaGaugetWidth;

        // Start is called before the first frame update
        void Awake()
        {
            _defaulStaminaGaugetWidth = _staminaGaugeFrontRect.sizeDelta.x;
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

                    //���삷��L�����N�^�[�̃X�^�~�i�Q�[�W�ɂ����A�X�^�~�i�Q�[�W��ύX�����鏈����ǉ�����B
                    //FhotonFusion��������AinputAuthority�����L�����N�^�[�݂̂Ɏw��
                    _playerStatuses[0].OnPlayerStaminaChange
                         .Subscribe(x =>
                         {
                             ChangeStaminaGauge(x);
                             if (x == 100)
                             { 
                                _staminaGaugeBackGround.DOFade(endValue: 0f, duration: 1f);
                                _staminaGaugeFrontImage.DOFade(endValue: 0f, duration: 1f);
                             }
                                 
                             else
                             { 
                                _staminaGaugeBackGround.DOFade(endValue: 1f, duration: 0f);
                                _staminaGaugeFrontImage.DOFade(endValue: 1f, duration: 0f);
                             }
                                 

                         }).AddTo(this);
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

        public void ChangeStaminaGauge(int value)
        {
            //  DoTween�̓����j��
            _staminaGaugeBackGround.DOKill();
            _staminaGaugeFrontImage.DOKill();

            //�X�^�~�i�̒l��0�`1�̒l�ɕ␳
            float fillAmount = (float)value / _playerStatuses[_myPlayerID].stamina_max;
            _staminaGaugeFrontRect.sizeDelta = new Vector2(_defaulStaminaGaugetWidth * fillAmount, _staminaGaugeFrontRect.sizeDelta.y);
        }
    }
}
