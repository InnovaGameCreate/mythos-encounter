using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using TMPro;
using Network;

namespace Scenes.CreateSession
{
    public class CreateSessionManager : MonoBehaviour
    {
        [SerializeField] private NetworkSystemConfig _networkSystemConfig;
        [Header("SessionNameLength")]
        [SerializeField] private float _sessionNameLengthMin = 1;
        [SerializeField] private float _sessionNameLengthMax = 12;
        [Header("CanvasObjects")]
        [SerializeField] private TMP_InputField _sessionNameInputField;
        [SerializeField] private Toggle _isPrivateToggle;

        public async void OnCreateSessionButton()
        {
            //セッション名が重複していないかを確認
            if (RunnerManager.Instance.sessionList.Exists(x => x.Name == _sessionNameInputField.text))
            {
                //Debug.Log("Exist Session Name");

                return;
            }

            //セッション名の文字数制限
            if (_sessionNameInputField.text.Length < _sessionNameLengthMin
                || _sessionNameInputField.text.Length > _sessionNameLengthMax)
            {
                //Debug.Log("Session Name Length Error");

                return;
            }

            //ボタンを触れられない状態にする
            var buttons = FindObjectsOfType<Button>();
            foreach (var button in buttons) button.interactable = false;

            //セッションのカスタムプロパティの作成
            var customProps = new Dictionary<string, SessionProperty>();
            customProps["visible"] = !_isPrivateToggle.isOn; //セッションを公開するかの設定

            //NetworkRunnerに渡すStartGameArgsの設定
            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.Host, //セッション内での権限
                Scene = SceneRef.FromIndex(_networkSystemConfig.inGameScene), //開始ゲームシーンの選択
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Runnerでのシーンの制御に必要
                SessionName = _sessionNameInputField.text, //セッション名
                SessionProperties = customProps, //セッションプロパティ
                PlayerCount = _networkSystemConfig.sessionPlayerMax, //最大人数の決定
                ConnectionToken = Guid.NewGuid().ToByteArray(), //プレイヤーのトークン(一時的)
            };

            //セッションに参加
            var result = await RunnerManager.Instance.JoinSession(startGameArgs);

            //セッションの作成に失敗したのでボタンロックを解除
            if (result == false) foreach (var button in buttons) button.interactable = true;
        }

        public void OnBackButton()
        {
            SceneManager.LoadScene("SessionLobby");
        }
    }
}
