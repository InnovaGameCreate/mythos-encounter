using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

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


        // Start is called before the first frame update
        void Awake()
        {
            _displayPlayerStatusManager.OnCompleteSort
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
                                _displayPlayerStatusManager.ChangeSliderValue(x, playerStatus.playerID, "Health");
                            }).AddTo(this);

                        playerStatus.OnPlayerSanValueChange
                            .Subscribe(x =>
                            {
                                //viewに反映
                                _displayPlayerStatusManager.ChangeSliderValue(x, playerStatus.playerID, "SanValue");
                            }).AddTo(this);
                    }
                }).AddTo(this);
        }
    }
}
