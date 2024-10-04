using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_3 : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        /// <summary>
        /// パブリックセッションへの参加
        /// </summary>
        public async void JoinPublicSession()
        {
            if (RunnerManager.Instance.SessionInfoList.Count == 0) return; //セッションが存在するかを判定

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //セッション内権限
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //セッション開始シーン
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
        }

        public void ToRoomSetting_4()
        {
            _uiManagerCs.ChangePanel(3);
        }
    }
}
