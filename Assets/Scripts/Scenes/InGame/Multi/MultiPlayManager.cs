using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Scenes.Ingame.Manager;
using UniRx;
using Scenes.Ingame.Stage;
using Scenes.Ingame.InGameSystem.UI;
using Scenes.Ingame.InGameSystem;

namespace Scenes.Ingame.Player
{
    public class MultiPlayManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] GameObject[] _spawnPoint;
        [SerializeField] GameObject _playerPrefab;

        
        [Networked][Capacity(4)]
        public NetworkLinkedList<NetworkId> playerNetworkList { get;}
        


        private ChangeDetector _changeDetector;
        public List<GameObject> PlayerList { get; private set; } = new List<GameObject>();
        public List<PlayerStatus> PlayerStatusList { get; private set; } = new List<PlayerStatus>();
        public void PlayerJoined(PlayerRef playerRef)
        {
            /*
            if (HasStateAuthority == false)
                return;

            var player = Runner.Spawn(_playerPrefab, GetSpawnPosition(), Quaternion.identity, playerRef);
            playerNetworkList.Add(player.Id);
            Runner.SetPlayerObject(playerRef, player.GetComponent<MultiPlayerMove>().Object);
            */
        }


        public void PlayerLeft(PlayerRef playerRef)
        {

        }

        private StageGenerator _stageGenerator;
        [SerializeField] private Vector3 _spawnPosition;
        [SerializeField] private GameObject _playerUI;

        public void Start()
        {
            IngameManager.Instance.OnStageGenerateEvent
                .Subscribe(_ =>
                {
                    _stageGenerator = FindObjectOfType<StageGenerator>();
                    Debug.Log("PlayerSpawnSuccess");
                    _spawnPosition = _stageGenerator.spawnPosition;

                    //沸く処理
                    var player = Runner.Spawn(_playerPrefab, _spawnPosition, Quaternion.identity, Runner.LocalPlayer);
                    playerNetworkList.Add(player.Id);
                    var playerUI = Instantiate(_playerUI, Vector3.zero, Quaternion.identity);
                    playerUI.transform.Find("FadeOut_InCanvas").GetComponent<FadeBlackImage>().SubscribeFadePanelEvent();//プレイヤーの死亡・蘇生時のイベントを登録
                    //プレイヤーの沸きが完了したことを知らせる
                    IngameManager.Instance.SetReady(ReadyEnum.PlayerReady);
                    Debug.LogWarning("プレイヤーの沸きが終了");
                }).AddTo(this);
        }
        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
        }

        Vector3 GetSpawnPosition()
        {
            var spawnPoint = _spawnPoint[Random.Range(0, _spawnPoint.Length)];
            //var randomPositionOffset = Random.insideUnitCircle * spawnPoint.Radius;
            return spawnPoint.transform.position;
        }

        /// <summary>
        /// ネットワークのシミュレーションごとに呼び出される。ロールバック等にも対応
        /// </summary>
        public override void FixedUpdateNetwork()
        {

            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(playerNetworkList):
                        List<GameObject> set = new List<GameObject>();
                        List<PlayerStatus> setCs = new List<PlayerStatus>();
                        for (int i = 0 ; i < playerNetworkList.Count; i++) {
                            if (Runner.TryFindObject(playerNetworkList[i], out var setObj)) {
                                set.Add(setObj.gameObject);
                                setCs.Add(setObj.gameObject.GetComponent<PlayerStatus>());
                            }
                        }
                        PlayerList = set;
                        PlayerStatusList = setCs;
                        break;
                        
                }
            }
        }
    }
}

