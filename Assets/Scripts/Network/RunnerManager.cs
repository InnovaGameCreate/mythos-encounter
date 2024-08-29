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

        public List<SessionInfo> sessionList = new List<SessionInfo>(); //����Photon�T�[�o�[�ő��݂��Ă���Z�b�V�����̃��X�g

        private void Awake()
        {
            //�C���X�^���X�쐬
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);

            if (Runner == null) Runner = GetComponent<NetworkRunner>();
            else Destroy(this.gameObject);
        }

        /// <summary>
        /// �Z�b�V�����ɎQ������
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="startGameArgs"></param>
        /// <returns></returns>
        public async Task<bool> JoinSession(StartGameArgs startGameArgs)
        {
            var result = await Runner.StartGame(startGameArgs); //�Z�b�V�����ɎQ������(�������Ԃ�������)

            if (result.Ok) //�Z�b�V�����Q���ɐ���
            {
                if (Runner.IsServer) //�Z�b�V�����Q�����Ƀz�X�g������U��ꂽ�ꍇ
                {
                    Debug.Log("Session Role : Host");
                }
                else //�Z�b�V�����Q�����ɃN���C�A���g������U��ꂽ�ꍇ
                {
                    Debug.Log("Session Role Client");
                }

                return true;
            }
            else //�Z�b�V�����Q���Ɏ��s
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
        /// ����Z�b�V�����̏�񂪍X�V���ꂽ�Ƃ��ɒ���Ăяo�����B
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="sessionList"></param> //���݂̃Z�b�V�������X�g
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            //�Z�b�V�������X�g�̍X�V
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
