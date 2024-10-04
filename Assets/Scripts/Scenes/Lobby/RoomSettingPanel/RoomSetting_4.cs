using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Common.Network;
using Fusion;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_4 : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _sessionName;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        public async void JoinPrivateSession()
        {
            //セッションが存在するかを判定
            string sessionName = _sessionName.text;
            var sessionInfo = RunnerManager.Instance.SessionInfoList.FirstOrDefault(x => x.Name == sessionName);
            if (sessionInfo == null) return;

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //セッション内権限
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //セッション開始シーン
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                SessionName = sessionName, //セッション名
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
        }
    }
}
