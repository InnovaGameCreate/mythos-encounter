using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_4 : MonoBehaviour
    {
        [SerializeField] private TMP_Text _sessionName;
        [SerializeField] private List<TMP_Text> _playerNames;

        private void Start()
        {
            this.FixedUpdateAsObservable(). //セッション名の更新
                Subscribe(_ => UpdateSessionName());

            this.FixedUpdateAsObservable(). //プレイヤーリストの更新
                Subscribe(_ => UpdatePlayerList());
        }

        /// <summary>
        /// セッション名の更新
        /// </summary>
        private void UpdateSessionName()
        {
            string sessionName = RunnerManager.Runner.SessionInfo.Name;
            _sessionName.text = "ルームID：" + sessionName;
        }

        /// <summary>
        /// プレイヤーリストの更新
        /// </summary>
        private void UpdatePlayerList()
        {
            var playerInfos = FindObjectsOfType<PlayerInfo>();

            for (int i = 0; i < _playerNames.Count; i++)
            {
                if (i < playerInfos.Length)
                {
                    _playerNames[i].text = playerInfos[i].userName;
                }
                else
                {
                    _playerNames[i].text = "メンバーがいません";
                }
            }
        }
    }
}
