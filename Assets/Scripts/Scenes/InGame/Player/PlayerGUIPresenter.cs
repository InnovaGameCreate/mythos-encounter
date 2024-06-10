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
        //Instance
        public static PlayerGUIPresenter Instance;

        //model
        [SerializeField] private PlayerStatus[] _playerStatuses;
        [SerializeField] private PlayerItem _playerItem;//�}���`�̎��̓X�N���v�g����inputAuthority�����Ă�player�̂���������
        [SerializeField] private PlayerInsanityManager _playerInsanityManager;//�}���`�̎��̓X�N���v�g����inputAuthority�����Ă�player�̂���������
        //View
        [SerializeField] private DisplayPlayerStatusManager _displayPlayerStatusManager;

        [Header("�Q�[����UI(�I�����C��)")]
        [SerializeField] private Slider[] _healthSliders;//�e�v���C���[��HP�o�[
        [SerializeField] private Slider[] _sanValueSliders;//�e�v���C���[��SAN�l�o�[
        [SerializeField] private TMP_Text[] _healthText;//�e�v���C���[��HP�c�ʕ\���e�L�X�g
        [SerializeField] private TMP_Text[] _sanValueText;//�e�v���C���[��SAN�l�c�ʕ\���e�L�X�g

        [Header("�Q�[����UI(�I�t���C��)")]
        [Header("�X�^�~�i�֌W")]//�X�^�~�i�Q�[�W�n
        [SerializeField] private Image _staminaGaugeBackGround;//�l�̃X�^�~�i�Q�[�W
        [SerializeField] private RectTransform _staminaGaugeFrontRect;//�l�̃X�^�~�i�Q�[�W
        [SerializeField] private Image _staminaGaugeFrontImage;//�l�̃X�^�~�i�Q�[�W

        [SerializeField] private GameObject _pop;//�A�C�e���|�b�v
        [SerializeField] private TMP_Text _pop_Text;//�A�C�e���|�b�v

        [Header("�A�C�e���֌W")]//�A�C�e���n
        [SerializeField] private Image[] _itemSlots;//�A�C�e���X���b�g(7��)
        [SerializeField] private Image[] _itemImages;//�A�C�e���̉摜(7��)

        [Header("�����֌W")]
        [SerializeField] private Image[] _insanityIcons;//�����A�C�R��(5��)
        [SerializeField] private Sprite[] _insanityIconSprites;//�����A�C�R���̌��摜.EyeParalyze,BodyParalyze,IncreasePulsation,Scream,Hallucination �̏��Ԃ�

        [Header("�����r���֌W")]
        [SerializeField] private Canvas _castGauge;
        [SerializeField] private Image _castGaugeImage;
        private int _myPlayerID = 0;

        //�X�^�~�i�Q�[�W�֘A�̃t�B�[���h
        private float _defaulStaminaGaugetWidth;

        // Start is called before the first frame update
        void Awake()
        {
            _castGauge.enabled = false;
            _defaulStaminaGaugetWidth = _staminaGaugeFrontRect.sizeDelta.x;

            //�v���C���[�̍쐬���I���A�z��̃\�[�g���I��������@�����
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


                    _playerStatuses[0].OnCastEvente
                    .Subscribe(time =>
                    {
                        _castGauge.enabled = true;
                        DOTween.Sequence()
                            .Append(_castGaugeImage.DOFillAmount(1, time))
                            .SetDelay(0.5f)
                            .Append(_castGaugeImage.DOFillAmount(0, 0))
                            .OnComplete(() => _castGauge.enabled = false);
                    }).AddTo(this);

                    //�A�C�e���֌W�̏����̒ǉ�
                    //PlayerItem�X�N���v�g�̎擾.�}���`�����̂Ƃ���inputAuthority�����L�����N�^�[�݂̂Ɏw��
                    _playerItem = GameObject.FindWithTag("Player").GetComponent<PlayerItem>();

                    //���ݑI������Ă���X���b�g�������\��
                    _playerItem.OnNowIndexChange
                        .Skip(1)
                        .Subscribe(x =>
                        {
                            //�S���̃X���b�g�̐F�����̊D�F�ɖ߂�
                            for (int i = 0; i < _itemSlots.Length; i++)
                            {
                                if (_playerItem.ItemSlots[i].myItemSlotStatus == ItemSlotStatus.available)
                                    _itemSlots[i].color = new Color(0.32f , 0.32f , 0.32f);
                            }
                            
                            //�I������Ă���X���b�g�����ԐF�ɕω�
                            _itemSlots[x].color = Color.red;
                        }).AddTo(this);

                    //�ڐ��̐�ɃA�C�e����StageIntract������ƃ|�b�v��\��������
                    _playerItem.OnPopActive
                        .Subscribe(x =>
                        {
                            if (x != null)
                            {
                                _pop_Text.text = x;
                                _pop.SetActive(true);
                            }
                            else
                            {
                                _pop_Text.text = null;
                                _pop.SetActive(false);
                            }

                        });

                    //�A�C�e���擾�E�j�����ɃA�C�e���X���b�g�̉摜��ύX������B
                    _playerItem.OnItemSlotReplace
                        .Subscribe(replaceEvent =>
                        {
                            if (_playerItem.ItemSlots[replaceEvent.Index].myItemData != null)
                            {
                                _itemImages[replaceEvent.Index].sprite = _playerItem.ItemSlots[replaceEvent.Index].myItemData.itemSprite;
                            }
                            else
                            {
                                _itemImages[replaceEvent.Index].sprite = null;

                            }

                            //���p�s�̃X���b�g�̘g��ɕω�
                            if (_playerItem.ItemSlots[replaceEvent.Index].myItemSlotStatus == ItemSlotStatus.unavailable)
                                _itemSlots[replaceEvent.Index].color = Color.blue;
                            else
                            { 
                                //��ɃO���[�ɖ߂�
                                _itemSlots[replaceEvent.Index].color = new Color(0.32f, 0.32f, 0.32f);

                                //�����I�𒆂̃A�C�e���X���b�g�Ȃ�ԐF�ɖ߂�
                                if(replaceEvent.Index == _playerItem.nowIndex)
                                    _itemSlots[replaceEvent.Index].color = Color.red;

                            }

                        }).AddTo(this);

                    //PlayerInsanityManager�X�N���v�g�̎擾.�}���`�����̂Ƃ���inputAuthority�����L�����N�^�[�݂̂Ɏw��
                    _playerInsanityManager = GameObject.FindWithTag("Player").GetComponent<PlayerInsanityManager>();

                    //�����̃X�N���v�g���Ǘ�����List�ɗv�f���ǉ����ꂽ�Ƃ��ɁA�A�C�R����ω�������B
                    _playerInsanityManager.OnInsanitiesAdd
                        .Subscribe(addEvent =>
                        {
                            _insanityIcons[addEvent.Index].color += new Color(0, 0, 0, 1.0f);

                            switch (_playerInsanityManager.Insanities[addEvent.Index])
                            {
                                case EyeParalyze:
                                    _insanityIcons[addEvent.Index].sprite = _insanityIconSprites[0];
                                    break;
                                case BodyParalyze:
                                    _insanityIcons[addEvent.Index].sprite = _insanityIconSprites[1];
                                    break;
                                case IncreasePulsation:
                                    _insanityIcons[addEvent.Index].sprite = _insanityIconSprites[2];
                                    break;
                                case Scream:
                                    _insanityIcons[addEvent.Index].sprite = _insanityIconSprites[3];
                                    break;
                                case Hallucination:
                                    _insanityIcons[addEvent.Index].sprite = _insanityIconSprites[4];
                                    break;
                                default:
                                    break;
                            }
                        }).AddTo(this);

                    //�����̃X�N���v�g���Ǘ�����List�̗v�f���폜���ꂽ�Ƃ��ɁA�A�C�R����ω�������B
                    _playerInsanityManager.OnInsanitiesRemove
                        .Subscribe(removeEvent =>
                        {
                            _insanityIcons[removeEvent.Index].color -= new Color(0, 0, 0, 1.0f);//�����ɂ���
                            _insanityIcons[removeEvent.Index].sprite = null;
                        }).AddTo(this);

                    //���]��Ԃɉ����ăA�C�R����ω�������B
                    _playerInsanityManager.OnPlayerBrainwashedChange
                        .Skip(1)//�������̎��͖���
                        .Subscribe(x =>
                        {
                            if (x)//���]��ԂɂȂ����Ƃ�
                            {
                                for (int i = 0; i < _insanityIcons.Length; i++)
                                    _insanityIcons[i].color -= new Color(0, 0, 0, 1.0f);//�����ɂ���
                            }
                            else//���]��Ԃ��������ꂽ�Ƃ�
                            {
                                for (int i = 0; i < _insanityIcons.Length; i++)
                                    _insanityIcons[i].color += new Color(0, 0, 0, 1.0f);//�s�����ɂ���
                            }
                        }).AddTo(this);
                        

                }).AddTo(this);

            //�C���X�^���X�̐ݒ�
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
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


        //�`�`�A�C�e���֘A�̏������L�q����ꏊ�`�`
    }
}
