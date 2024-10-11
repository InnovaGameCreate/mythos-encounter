using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_2 : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private int _playerNum = 4;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        public void ToRoomSetting_3()
        {
            _uiManagerCs.ChangePanel(2);
            SessionCreate(); //パブリックセッション作成
        }

        public void ToRoomSetting_4()
        {
            _uiManagerCs.ChangePanel(3);
            SessionCreate(Guid.NewGuid().ToString()); //プライベートセッション作成
        }

        /// <summary>
        /// セッション作成
        /// </summary>
        private async void SessionCreate()
        {
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //セッション内権限
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(),
                PlayerCount = _playerNum, //最大人数の決定
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
        }

        /// <summary>
        /// セッション作成（プライベート）
        /// </summary>
        /// <param name="sessionName"></param>
        private async void SessionCreate(string sessionName)
        {
            Debug.Log("Session Name : " + sessionName);

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //セッション内権限
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                SessionName = sessionName, //セッション名
                PlayerCount = _playerNum, //最大人数の決定
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
        }
    }
}
