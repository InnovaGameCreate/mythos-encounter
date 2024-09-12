using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UniRx;
using Network;

public class InLobbyManager : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    public override void Spawned()
    {
        if (Object.HasStateAuthority == false) return;

        if (Object.IsResume == false)
        {
            foreach (var player in RunnerManager.Instance.PlayerList.Keys)
                PlayerSpawned(player);
        }

        RunnerManager.Instance.NewPlayerJoinedCall.Subscribe(
            player => PlayerSpawned(player)).AddTo(this);
    }

    private void PlayerSpawned(PlayerRef player)
    {
        var playerObj = RunnerManager.Instance.PlayerSpawned(
            _playerPrefab,
            new Vector3(0, Random.Range(0, 100), 0),
            Quaternion.identity,
            player);
        var playerInfo = playerObj.GetComponent<PlayerInfo>();
        playerInfo.debugNumber = Random.Range(0, 100);
    }
}
