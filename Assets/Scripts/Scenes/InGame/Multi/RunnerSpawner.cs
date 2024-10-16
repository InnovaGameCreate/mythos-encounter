using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class RunnerSpawner : MonoBehaviour
{
    public NetworkRunner RunnerPrefabs;
    NetworkRunner _runnerInstance;

    async void StartGame(GameMode mode)
    {
        _runnerInstance = Instantiate(RunnerPrefabs);

        //予期しないシャットダウンを処理できるようにシャットダウン用のリスナーを設定
        var events = _runnerInstance.GetComponent<NetworkEvents>();
        events.OnShutdown.AddListener(OnShutdown);

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

 
    }

    void OnGUI()
    {
        if (_runnerInstance == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
            if (GUI.Button(new Rect(0, 80, 200, 40), "Single"))
            {
                StartGame(GameMode.Single);
            }
        }
    }

    public async Task Disconnect()
    {
        if (_runnerInstance == null)
            return;

        // Remove shutdown listener since we are disconnecting deliberately
        var events = _runnerInstance.GetComponent<NetworkEvents>();
        events.OnShutdown.RemoveListener(OnShutdown);

        await _runnerInstance.Shutdown();
        _runnerInstance = null;

        // Reset of scene network objects is needed, reload the whole scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        // Unexpected shutdown happened (e.g. Host disconnected)

        // Reset of scene network objects is needed, reload the whole scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
