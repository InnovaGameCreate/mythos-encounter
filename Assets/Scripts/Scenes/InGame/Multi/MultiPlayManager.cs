using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Fusion;

namespace Scenes.Ingame.Player
{
    public class MultiPlayManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] GameObject[] _spawnPoint;
        [SerializeField] GameObject _playerPrefab;

        public void PlayerJoined(PlayerRef playerRef)
        {
            if (HasStateAuthority == false)
                return;

            var player = Runner.Spawn(_playerPrefab, GetSpawnPosition(), Quaternion.identity, playerRef);
            Runner.SetPlayerObject(playerRef, player.GetComponent<MultiPlayerMove>().Object);
        }

        public void PlayerLeft(PlayerRef playerRef)
        {

        }

        Vector3 GetSpawnPosition()
        {
            var spawnPoint = _spawnPoint[Random.Range(0, _spawnPoint.Length)];
            //var randomPositionOffset = Random.insideUnitCircle * spawnPoint.Radius;
            return spawnPoint.transform.position;
        }
    }
}

