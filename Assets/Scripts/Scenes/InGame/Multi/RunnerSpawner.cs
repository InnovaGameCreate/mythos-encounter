using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Scenes.Ingame.Stage;

public class RunnerSpawner : MonoBehaviour, IPlayerJoined
{
    public NetworkRunner RunnerPrefabs;
    NetworkRunner _runnerInstance;
    [SerializeField]
    ReadyFlagManager _readyFlag;

    async void StartGame(GameMode mode)
    {
        _runnerInstance = Instantiate(RunnerPrefabs);
        _readyFlag = _runnerInstance.GetComponent<ReadyFlagManager>();
        //予期しないシャットダウンを処理できるようにシャットダウン用のリスナーを設定
        var events = _runnerInstance.GetComponent<NetworkEvents>();
        events.OnShutdown.AddListener(OnShutdown);

        //RoomDataHolder.AddListener(events);

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

        //await _readyFlag.WaitClientsReady();
    }

    public void PlayerJoined(PlayerRef playerRef)
    {
        _readyFlag.PlayerNum++;
        Debug.Log(_readyFlag.PlayerNum);
    }

    public void Action()
    {
        try
        {
            _runnerInstance.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
        }
        catch
        {
            Debug.LogError("シーン遷移で問題が発生しました。");
            throw;
        }
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
