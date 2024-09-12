using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Network;

public class StartManager : MonoBehaviour
{
    [SerializeField] private Button _matchingButton;
    [SerializeField] private NetworkRunner _runnerPrefab;

    public async void OnMatchingButton()
    {
        ButtonLock(true);

        var runner = Instantiate(_runnerPrefab);
        runner.ProvideInput = true;
        DontDestroyOnLoad(runner);

        var args = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            Scene = SceneRef.FromIndex(1),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
            SessionName = "test",
            PlayerCount = 2,
            ConnectionToken = Guid.NewGuid().ToByteArray(),
        };

        var result = await RunnerManager.Instance.JoinSession(args);

        if (result == false) ButtonLock(false);
    }

    private void ButtonLock(bool state)
    {
        _matchingButton.interactable = !state;
    }
}
