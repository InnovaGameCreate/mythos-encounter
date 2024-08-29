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

        public async void ToSessionLobbyButton()
        {
            //全てのButtonを触れない状態にする
            var buttons = FindObjectsOfType<Button>();
            foreach (var button in buttons) button.interactable = false;

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
                foreach (var button in buttons) button.interactable = true; //Interactableを解除
            }
        }
    }
}
