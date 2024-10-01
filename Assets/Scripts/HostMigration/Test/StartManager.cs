using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StartManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner _runnerPrefab;
    public async void OnButton()
    {
        var runner = Instantiate(_runnerPrefab);
        runner.ProvideInput = true;

        var args = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            Scene = SceneRef.FromIndex(1),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
            SessionName = "test",
            PlayerCount = 2,
            ConnectionToken = Guid.NewGuid().ToByteArray(),
        };

        await runner.StartGame(args);
    }
}
