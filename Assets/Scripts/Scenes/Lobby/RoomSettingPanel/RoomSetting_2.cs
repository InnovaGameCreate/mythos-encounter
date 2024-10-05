using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_2 : MonoBehaviour
    {
        private static readonly int SESSION_NUMBER_MAX = 999999;
        [SerializeField] private Button _publicButton;
        [SerializeField] private Button _privateButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private int _maxPlayer = 4;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        /// <summary>
        /// セッション作成（パブリック）
        /// </summary>
        public async void CreatePublicSession()
        {
            ButtonControl(false); //ボタンロック

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //セッション内権限
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //セッション開始シーン
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                PlayerCount = _maxPlayer, //最大人数の決定
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
            if (result.Ok == false)
            {
                ButtonControl(true); //ボタンロック解除
            }
        }

        /// <summary>
        /// セッション作成（プライベート）
        /// </summary>
        /// <param name="sessionName"></param>
        public async void CreatePrivateSession()
        {
            ButtonControl(false); //ボタンロック

            string sessionId = GetSessionId(); //セッションIDの取得

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //セッション内権限
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //セッション開始シーン
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion用のシーン遷移を管理するコンポーネント
                SessionName = sessionId, //セッション名
                PlayerCount = _maxPlayer, //最大人数の決定
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーの接続トークン
            };

            var result = await RunnerManager.Runner.StartGame(args); //セッション開始
            if (result.Ok == false)
            {
                ButtonControl(true); //ボタンロック解除
            }
        }

        /// <summary>
        /// ボタンのロック制御
        /// </summary>
        /// <param name="state"></param>
        private void ButtonControl(bool state)
        {
            _publicButton.interactable = state;
            _privateButton.interactable = state;
            _closeButton.interactable = state;
        }

        /// <summary>
        /// プライベートのセッションIDの取得
        /// </summary>
        /// <returns></returns>
        private string GetSessionId()
        {
            string sessionId = "";

            while (true)
            {
                //ID生成
                int randomNumber = UnityEngine.Random.Range(0, SESSION_NUMBER_MAX);
                string sessionIdTmp = randomNumber.ToString("D6");

                //重複確認
                var result = RunnerManager.Instance.SessionInfoList.FirstOrDefault(x => x.Name == sessionIdTmp);
                if (result == null)
                {
                    sessionId = sessionIdTmp;
                    break;
                }
            }

            Debug.LogError("Session Id : " + sessionId);
            return sessionId;
        }
    }
}
