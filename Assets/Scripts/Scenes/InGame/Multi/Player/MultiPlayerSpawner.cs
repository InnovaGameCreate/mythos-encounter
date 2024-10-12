using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading;
using Scenes.Ingame.InGameSystem;
using Scenes.Ingame.InGameSystem.UI;
using Scenes.Ingame.Manager;
using Scenes.Ingame.Player;
using Scenes.Ingame.Stage;
using System.Collections.Specialized;
using UniRx;
using System.CodeDom;

namespace Scenes.Ingame.Multi.Player
{
    public class MultiPlayerSpawner : NetworkBehaviour
    {
        [SerializeField]
        NetworkPrefabRef _playerPrefab;
        StageGenerator _stageGenerator;
        Vector3 _spawnPosition;
        NetworkRunner runner;

        void Start()
        {
            IngameManager.Instance.OnStageGenerateEvent
                .Subscribe(_ =>
                {
                    _stageGenerator = FindObjectOfType<StageGenerator>();
                    Debug.Log("PlayerSpawnSuccess");
                    _spawnPosition = _stageGenerator.spawnPosition;
                    SpawnPlayer();
                }).AddTo(this);

            if(Runner == null)
            {
                runner = FindObjectOfType<NetworkRunner>();
            }
        }

        void SpawnPlayer()
        {
            if (HasStateAuthority == false)
                return;

            foreach(PlayerRef playerRef in Runner.ActivePlayers)
            {
                var player = Runner.Spawn(_playerPrefab, _spawnPosition, Quaternion.identity, playerRef);
                Runner.SetPlayerObject(playerRef, player.GetComponent<MultiPlayerMove>().Object);
            }
            
        }
    }
}

