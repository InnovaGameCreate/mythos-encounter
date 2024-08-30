using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

namespace Network
{
    public class Develop_ToMatchingSceneManager : MonoBehaviour
    {
        [SerializeField] private NetworkRunner _runnerPrefab; //NetworkRunnerのPrefab
        [Header("Canvas Objects")]
        [SerializeField] private Button _toSessionLobbyButton;

        public async void ToSessionLobbyButton()
        {
            //全てのButtonを触れない状態にする
            AllInteractable(false);

            //NetworkRunnerを配置する
            var runner = Instantiate(_runnerPrefab);
            runner.ProvideInput = true; //入力権限(InputAuthority)を使う
            DontDestroyOnLoad(runner); //DontDestroyを付けないとシーン遷移時に消滅する(HostMigration有効時)

            //ロビーへの参加
            var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);
            if (result.Ok)
            {
                Debug.Log("Join SessionLobby");
                SceneManager.LoadScene("SessionLobby"); //SessionLobbyシーンへの遷移
            }
            else
            {
                Debug.LogError($"Error : {result.ShutdownReason}"); //接続の失敗
                AllInteractable(true); //ロックを解除
            }
        }

        /// <summary>
        /// 全てのUIオブジェクトのInteractableを操作する
        /// </summary>
        /// <param name="state"></param>
        public void AllInteractable(bool state)
        {
            _toSessionLobbyButton.interactable = state;
        }
    }
}
