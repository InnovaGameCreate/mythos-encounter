using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using Common.Network;
using Common.UI;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_4 : MonoBehaviour
    {
        [SerializeField] private Button _enterRoomButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_InputField _sessionName;
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private Dialog _dialog;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        public async void JoinPrivateSession()
        {
            //セッションが存在するかを判定
            string sessionName = _sessionName.text;
            var sessionInfo = RunnerManager.Instance.SessionInfoList.FirstOrDefault(x => x.Name == sessionName);
            if (sessionInfo == null)
            {
                var dialog = Instantiate(_dialog, _uiManagerCs.transform);
                dialog.Init("そのIDのルームは存在しません。");
                return;
            }

            ButtonControl(false); //ボタンロック

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //セッション内権限
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //セッション開始シーン
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                SessionName = sessionName, //セッション名
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
            if (result.Ok == false)
            {
                ButtonControl(true); //ボタンロック解除
            }
        }

        /// <summary>
        /// ボタンロック制御
        /// </summary>
        /// <param name="state"></param>
        private void ButtonControl(bool state)
        {
            _enterRoomButton.interactable = state;
            _closeButton.interactable = state;
        }
    }
}
