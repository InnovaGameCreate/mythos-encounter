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
    /// PlayerStatusとDisplayPlayerStatusManagerの橋渡しを行うクラス
    /// MV(R)PにおけるPresenterの役割を想定
    /// </summary>
    public class PlayerGUIPresenter : MonoBehaviour
    {
        //Instance
        public static PlayerGUIPresenter Instance;

        //model
        [SerializeField] private PlayerStatus[] _playerStatuses;
        [SerializeField] private PlayerItem _playerItem;//マルチの時はスクリプト内でinputAuthority持ってるplayerのを代入させる
        [SerializeField] private PlayerInsanityManager _playerInsanityManager;//マルチの時はスクリプト内でinputAuthority持ってるplayerのを代入させる
        //View
        [SerializeField] private DisplayPlayerStatusManager _displayPlayerStatusManager;

        [Header("ゲーム内UI(オンライン)")]
        [SerializeField] private Slider[] _healthSliders;//各プレイヤーのHPバー
        [SerializeField] private Slider[] _sanValueSliders;//各プレイヤーのSAN値バー
        [SerializeField] private TMP_Text[] _healthText;//各プレイヤーのHP残量表示テキスト
        [SerializeField] private TMP_Text[] _sanValueText;//各プレイヤーのSAN値残量表示テキスト

        [Header("ゲーム内UI(オフライン)")]
        [Header("スタミナ関係")]//スタミナゲージ系
        [SerializeField] private Image _staminaGaugeBackGround;//個人のスタミナゲージ
        [SerializeField] private RectTransform _staminaGaugeFrontRect;//個人のスタミナゲージ
        [SerializeField] private Image _staminaGaugeFrontImage;//個人のスタミナゲージ

        [SerializeField] private GameObject _pop;//アイテムポップ
        [SerializeField] private TMP_Text _pop_Text;//アイテムポップ

        [Header("アイテム関係")]//アイテム系
        [SerializeField] private Image[] _itemSlots;//アイテムスロット(7個)
        [SerializeField] private Image[] _itemImages;//アイテムの画像(7個)

        [Header("発狂関係")]
        [SerializeField] private Image[] _insanityIcons;//発狂アイコン(5個)
        [SerializeField] private Sprite[] _insanityIconSprites;//発狂アイコンの元画像.EyeParalyze,BodyParalyze,IncreasePulsation,Scream,Hallucination の順番で

        [Header("呪文詠唱関係")]
        [SerializeField] private Canvas _castGauge;
        [SerializeField] private Image _castGaugeImage;
        private int _myPlayerID = 0;

        //スタミナゲージ関連のフィールド
        private float _defaulStaminaGaugetWidth;

        // Start is called before the first frame update
        void Awake()
        {
            _castGauge.enabled = false;
            _defaulStaminaGaugetWidth = _staminaGaugeFrontRect.sizeDelta.x;

            //プレイヤーの作成が終わり、配列のソートが終了したら叩かれる
            _displayPlayerStatusManager.OnCompleteSort
                .FirstOrDefault()
                .Subscribe(_ => 
                {
                    //DisplayPlayerStatusManagerにあるソート済みの配列を取得
                    _playerStatuses = _displayPlayerStatusManager.PlayerStatuses;

                    //各プレイヤーのHPやSAN値が変更されたときの処理を追加する。
                    foreach (var playerStatus in _playerStatuses)
                    {
                        playerStatus.OnPlayerHealthChange
                            .Subscribe(x =>
                            {
                                //viewに反映
                                ChangeSliderValue(x, playerStatus.playerID, "Health");
                            }).AddTo(this);

                        playerStatus.OnPlayerSanValueChange
                            .Subscribe(x =>
                            {
                                //viewに反映
                                ChangeSliderValue(x, playerStatus.playerID, "SanValue");
                            }).AddTo(this);
                    }

                    //操作するキャラクターのスタミナゲージにだけ、スタミナゲージを変更させる処理を追加する。
                    //FhotonFusionだったら、inputAuthorityを持つキャラクターのみに指定
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

                    //アイテム関係の処理の追加
                    //PlayerItemスクリプトの取得.マルチ実装のときはinputAuthorityを持つキャラクターのみに指定
                    _playerItem = GameObject.FindWithTag("Player").GetComponent<PlayerItem>();

                    //現在選択されているスロットを強調表示
                    _playerItem.OnNowIndexChange
                        .Skip(1)
                        .Subscribe(x =>
                        {
                            //全部のスロットの色を元の灰色に戻す
                            for (int i = 0; i < _itemSlots.Length; i++)
                            {
                                if (_playerItem.ItemSlots[i].myItemSlotStatus == ItemSlotStatus.available)
                                    _itemSlots[i].color = new Color(0.32f , 0.32f , 0.32f);
                            }
                            
                            //選択されているスロットだけ赤色に変化
                            _itemSlots[x].color = Color.red;
                        }).AddTo(this);

                    //目線の先にアイテムかStageIntractがあるとポップを表示させる
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

                    //アイテム取得・破棄時にアイテムスロットの画像を変更させる。
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

                            //利用不可のスロットの枠を青に変化
                            if (_playerItem.ItemSlots[replaceEvent.Index].myItemSlotStatus == ItemSlotStatus.unavailable)
                                _itemSlots[replaceEvent.Index].color = Color.blue;
                            else
                            { 
                                //先にグレーに戻す
                                _itemSlots[replaceEvent.Index].color = new Color(0.32f, 0.32f, 0.32f);

                                //もし選択中のアイテムスロットなら赤色に戻す
                                if(replaceEvent.Index == _playerItem.nowIndex)
                                    _itemSlots[replaceEvent.Index].color = Color.red;

                            }

                        }).AddTo(this);

                    //PlayerInsanityManagerスクリプトの取得.マルチ実装のときはinputAuthorityを持つキャラクターのみに指定
                    _playerInsanityManager = GameObject.FindWithTag("Player").GetComponent<PlayerInsanityManager>();

                    //発狂のスクリプトを管理するListに要素が追加されたときに、アイコンを変化させる。
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

                    //発狂のスクリプトを管理するListの要素が削除されたときに、アイコンを変化させる。
                    _playerInsanityManager.OnInsanitiesRemove
                        .Subscribe(removeEvent =>
                        {
                            _insanityIcons[removeEvent.Index].color -= new Color(0, 0, 0, 1.0f);//透明にする
                            _insanityIcons[removeEvent.Index].sprite = null;
                        }).AddTo(this);

                    //洗脳状態に応じてアイコンを変化させる。
                    _playerInsanityManager.OnPlayerBrainwashedChange
                        .Skip(1)//初期化の時は無視
                        .Subscribe(x =>
                        {
                            if (x)//洗脳状態になったとき
                            {
                                for (int i = 0; i < _insanityIcons.Length; i++)
                                    _insanityIcons[i].color -= new Color(0, 0, 0, 1.0f);//透明にする
                            }
                            else//洗脳状態が解除されたとき
                            {
                                for (int i = 0; i < _insanityIcons.Length; i++)
                                    _insanityIcons[i].color += new Color(0, 0, 0, 1.0f);//不透明にする
                            }
                        }).AddTo(this);
                        

                }).AddTo(this);

            //インスタンスの設定
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        /// <summary>
        /// Sliderの値を変える為の関数.Slider,Textに直接参照している
        /// </summary>
        /// <param name="value">Slinder.Valueに代入する値</param>
        /// <param name="ID">プレイヤーID</param>
        /// <param name="mode">Health(体力), SanValue(SAN値)どちらを変更するのかを決定</param>
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
            //  DoTweenの動作を破棄
            _staminaGaugeBackGround.DOKill();
            _staminaGaugeFrontImage.DOKill();

            //スタミナの値を0～1の値に補正
            float fillAmount = (float)value / _playerStatuses[_myPlayerID].stamina_max;
            _staminaGaugeFrontRect.sizeDelta = new Vector2(_defaulStaminaGaugetWidth * fillAmount, _staminaGaugeFrontRect.sizeDelta.y);
        }


        //～～アイテム関連の処理を記述する場所～～
    }
}
