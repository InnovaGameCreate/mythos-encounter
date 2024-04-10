using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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
        //View
        [SerializeField] private DisplayPlayerStatusManager _displayPlayerStatusManager;

        //画面内のUIの列挙
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
    }
}
