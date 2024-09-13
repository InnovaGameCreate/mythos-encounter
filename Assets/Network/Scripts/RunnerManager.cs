using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using UniRx;

namespace Network
{
    public class RunnerManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private Config _configAsset; //ネットワークシステムのコンフィグ

        public static NetworkRunner Runner;
        public static RunnerManager Instance;
        public IObservable<PlayerRef> NewPlayerJoinedCall { get { return _newPlayerJoinedSubject; } } //新しいプレイヤーが参加したときにコールされる
        public Dictionary<PlayerRef, NetworkObject> PlayerList;

        private Subject<PlayerRef> _newPlayerJoinedSubject = new Subject<PlayerRef>();

        private void Awake()
        {
            if (Runner == null) Runner = GetComponent<NetworkRunner>();
            else Destroy(this.gameObject);

            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);
        }

        /// <summary>
        /// セッションに参加する
        /// </summary>
        /// <param name="args"></param> StartGameArgs
        /// <returns></returns>
        public async Task<bool> JoinSession(StartGameArgs args)
        {
            var result = await Runner.StartGame(args);

            if (result.Ok)
            {
                if (Runner.IsServer)
                {
                    Debug.Log("Session Role : Host");
                    PlayerList = new Dictionary<PlayerRef, NetworkObject>(); //ホストはプレイヤーリストを管理する
                }
                else
                {
                    Debug.Log("Session Role : Client");
                }

                return true;
            }
            else
            {
                Debug.LogError($"Error : {result.ShutdownReason}");
                return false;
            }
        }

        /// <summary>
        /// 入力権限を付与するオブジェクトをスポーンする
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public NetworkObject PlayerSpawned(GameObject prefab, Vector3 position, Quaternion rotation, PlayerRef player)
        {
            if (!Runner.IsServer) return null;

            //プレイヤオブジェクトをスポーンさせる
            var playerObj = Runner.Spawn(prefab, position, rotation, player, (_, obj) =>
            {
                if (_configAsset.useHostMigration)
                {
                    //接続トークンの設定
                    if (obj.TryGetComponent<ConnectionToken>(out var connectionToken))
                        connectionToken.token = new Guid(Runner.GetPlayerConnectionToken(player)).GetHashCode();

                    //オブジェクトトークンの設定
                    if (obj.TryGetComponent<ObjectToken>(out var objectToken))
                    {
                        if (Runner.LocalPlayer == player) objectToken.token = "HOST";
                        else objectToken.token = Guid.NewGuid().ToString();
                    }
                }
            });

            //プレイヤーリストに追加
            PlayerList.Add(player, playerObj);

            return playerObj;
        }

        /// <summary>
        /// 入力権限がないオブジェクトをスポーンする
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public NetworkObject ObjectSpawned(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!Runner.IsServer) return null;

            //オブジェクトをスポーンさせる
            var networkObj = Runner.Spawn(prefab, position, rotation, null, (_, obj) =>
            {
                if (_configAsset.useHostMigration)
                {
                    //オブジェクトトークンの設定
                    if (obj.TryGetComponent<ObjectToken>(out var objectToken))
                        objectToken.token = Guid.NewGuid().ToString();
                }
            });

            return networkObj;
        }

        /// <summary>
        /// プレイヤーの参加
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="player"></param>
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;

            if (_configAsset.useHostMigration)
            {
                //既存プレイヤーかを判別する
                int connectionToken = new Guid(runner.GetPlayerConnectionToken(player)).GetHashCode(); //接続トークンの読み出し
                var playerList = FindObjectsOfType<ConnectionToken>();
                var isResumePlayer = playerList.FirstOrDefault(player => player.token == connectionToken);

                if (isResumePlayer == null) //新規プレイヤー
                {
                    //新しいプレイヤーが参加
                    _newPlayerJoinedSubject.OnNext(player);
                }
                else //既存プレイヤー
                {
                    //入力権限を既存プレイヤーに付与
                    isResumePlayer.Object.AssignInputAuthority(player);

                    //ホストのプレイヤーオブジェクトにはHOSTフラグを付与
                    if (isResumePlayer.TryGetComponent<NetworkObject>(out var playerObj))
                    {
                        if (playerObj.InputAuthority.PlayerId == runner.LocalPlayer.PlayerId
                            && playerObj.TryGetComponent<ObjectToken>(out var objectToken))
                        {
                            objectToken.token = "HOST";
                        }
                    }

                    //プレイヤーリストに追加
                    PlayerList.Add(player, playerObj);
                }
            }
            else
            {
                //新しいプレイヤーが参加
                _newPlayerJoinedSubject.OnNext(player);
            }
        }

        /// <summary>
        /// プレイヤーの退出
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="player"></param>
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;

            if (PlayerList.TryGetValue(player, out NetworkObject playerObj))
            {
                runner.Despawn(playerObj);
                PlayerList.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        /// <summary>
        /// ホスト遷移開始時に呼び出される
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="hostMigrationToken"></param>
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            if (!_configAsset.useHostMigration) return;

            var handler = Instantiate(_configAsset.hostMigrationHandler);
            handler.RebootRunner(_configAsset.runner, runner, hostMigrationToken);
        }

        /// <summary>
        /// シーン遷移完了後に呼び出される
        /// </summary>
        /// <param name="runner"></param>
        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (!runner.IsServer || runner.IsResume) return;

            //現在のシーンインデックスのシーンマネージャーを取得する
            if (_configAsset.sceneManagerTables.TryGetValue(SceneManager.GetActiveScene().buildIndex, out var sceneManagerPrefab))
            {
                //シーンマネージャーをスポーンさせる
                ObjectSpawned(sceneManagerPrefab, Vector3.zero, Quaternion.identity);
            }
        }

        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    }
}
