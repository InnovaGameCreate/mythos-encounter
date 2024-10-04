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
        [SerializeField] private int _maxPlayer = 4;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        /// <summary>
        /// セッション作成（パブリック）
        /// </summary>
        public async void CreatePublicSession()
        {
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //セッション内権限
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //セッション開始シーン
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                PlayerCount = _maxPlayer, //最大人数の決定
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
        }

        /// <summary>
        /// セッション作成（プライベート）
        /// </summary>
        /// <param name="sessionName"></param>
        public async void CreatePrivateSession()
        {
            string sessionName = Guid.NewGuid().ToString();
            Debug.Log("Session Name : " + sessionName);

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //セッション内権限
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //セッション開始シーン
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                SessionName = sessionName, //セッション名
                PlayerCount = _maxPlayer, //最大人数の決定
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
        }
    }
}
