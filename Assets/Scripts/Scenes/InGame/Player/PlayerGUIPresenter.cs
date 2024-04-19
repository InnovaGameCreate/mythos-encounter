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
        //model
        [SerializeField] private PlayerStatus[] _playerStatuses;
        [SerializeField] private PlayerItem _playerItem;//マルチの時はスクリプト内でinputAuthority持ってるplayerのを代入させる
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

        [Header("アイテム関係")]//アイテム系
        [SerializeField] private Image[] _itemSlots;//アイテムスロット(7個)
        [SerializeField] private Image[] _itemImages;//アイテムの画像(7個)
        [SerializeField] private GameObject _itemPop;//アイテムポップ
        [SerializeField] private TMP_Text _itemPop_Text;//アイテムポップ

        private int _myPlayerID = 0;

        //スタミナゲージ関連のフィールド
        private float _defaulStaminaGaugetWidth;

        // Start is called before the first frame update
        void Awake()
        {
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

                    //アイテム関係の処理の追加
                    //PlayerItemスクリプトの取得.マルチ実装のときはinputAuthorityを持つキャラクターのみに指定
                    _playerItem = GameObject.FindWithTag("Player").GetComponent<PlayerItem>();

                    //現在選択されているスロットを強調表示
                    _playerItem.OnNowIndexChange
                        .Subscribe(x =>
                        {
                            //全部のスロットの色を元の灰色に戻す
                            for(int i = 0; i< _itemSlots.Length; i++)
                                _itemSlots[i].color = new Color(84,84,84);

                            //選択されているスロットだけ赤色に変化
                            _itemSlots[x].color = Color.red;
                        }).AddTo(this);

                    //目線の先にアイテムがあるとアイテムポップを表示させる
                    _playerItem.OnItemPopActive
                        .Subscribe(x =>
                        {
                            if (x != null)
                            {
                                _itemPop_Text.text = x;
                                _itemPop.SetActive(true);
                            }
                            else
                            {
                                _itemPop_Text.text = null;
                                _itemPop.SetActive(false);
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
                        }).AddTo(this);

                }).AddTo(this);
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
