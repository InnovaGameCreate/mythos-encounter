using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

namespace Network
{
    public class RunnerManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static RunnerManager Instance;
        public static NetworkRunner Runner;

        public List<SessionInfo> sessionList = new List<SessionInfo>(); //現在Photonサーバーで存在しているセッションのリスト

        private void Awake()
        {
            //インスタンス作成
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);

            if (Runner == null) Runner = GetComponent<NetworkRunner>();
            else Destroy(this.gameObject);
        }

        /// <summary>
        /// セッションに参加する
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="startGameArgs"></param>
        /// <returns></returns>
        public async Task<bool> JoinSession(StartGameArgs startGameArgs)
        {
            var result = await Runner.StartGame(startGameArgs); //セッションに参加する(少し時間がかかる)

            if (result.Ok) //セッション参加に成功
            {
                if (Runner.IsServer) //セッション参加時にホストが割り振られた場合
                {
                    Debug.Log("Session Role : Host");
                }
                else //セッション参加時にクライアントが割り振られた場合
                {
                    Debug.Log("Session Role Client");
                }

                return true;
            }
            else //セッション参加に失敗
            {
                Debug.LogError($"Error : {result.ShutdownReason}");
                return false;
            }
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        /// <summary>
        /// あるセッションの情報が更新されたときに逐一呼び出される。
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="sessionList"></param> //現在のセッションリスト
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            //セッションリストの更新
            this.sessionList = new List<SessionInfo>(sessionList);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    }
}
