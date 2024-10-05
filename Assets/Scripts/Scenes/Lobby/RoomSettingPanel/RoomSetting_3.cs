using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Common.Network;
using Common.UI;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_3 : MonoBehaviour
    {
        [SerializeField] private Button _publicButton;
        [SerializeField] private Button _privateButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private Dialog _dialog;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        /// <summary>
        /// パブリックセッションへの参加
        /// </summary>
        public async void JoinPublicSession()
        {
            //セッションが存在するかを判定
            if (RunnerManager.Instance.SessionInfoList.Count == 0)
            {
                var dialog = Instantiate(_dialog, _uiManagerCs.transform);
                dialog.Init("ルームが存在しません");
                return;
            }

            ButtonControl(false); //ボタンロック

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //セッション内権限
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //セッション開始シーン
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
            if (result.Ok == false)
            {
                ButtonControl(true); //ボタンロック解除
            }
        }

        public void ToRoomSetting_4()
        {
            _uiManagerCs.ChangePanel(3); //プライベート参加画面
        }

        /// <summary>
        /// ボタンロック制御
        /// </summary>
        /// <param name="state"></param>
        private void ButtonControl(bool state)
        {
            _publicButton.interactable = state;
            _privateButton.interactable = state;
            _closeButton.interactable = state;
        }
    }
}
