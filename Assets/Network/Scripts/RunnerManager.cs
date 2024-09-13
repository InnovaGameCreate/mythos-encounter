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
        [SerializeField] private Config _configAsset; //�l�b�g���[�N�V�X�e���̃R���t�B�O

        public static NetworkRunner Runner;
        public static RunnerManager Instance;
        public IObservable<PlayerRef> NewPlayerJoinedCall { get { return _newPlayerJoinedSubject; } } //�V�����v���C���[���Q�������Ƃ��ɃR�[�������
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
        /// �Z�b�V�����ɎQ������
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
                    PlayerList = new Dictionary<PlayerRef, NetworkObject>(); //�z�X�g�̓v���C���[���X�g���Ǘ�����
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
        /// ���͌�����t�^����I�u�W�F�N�g���X�|�[������
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public NetworkObject PlayerSpawned(GameObject prefab, Vector3 position, Quaternion rotation, PlayerRef player)
        {
            if (!Runner.IsServer) return null;

            //�v���C���I�u�W�F�N�g���X�|�[��������
            var playerObj = Runner.Spawn(prefab, position, rotation, player, (_, obj) =>
            {
                if (_configAsset.useHostMigration)
                {
                    //�ڑ��g�[�N���̐ݒ�
                    if (obj.TryGetComponent<ConnectionToken>(out var connectionToken))
                        connectionToken.token = new Guid(Runner.GetPlayerConnectionToken(player)).GetHashCode();

                    //�I�u�W�F�N�g�g�[�N���̐ݒ�
                    if (obj.TryGetComponent<ObjectToken>(out var objectToken))
                    {
                        if (Runner.LocalPlayer == player) objectToken.token = "HOST";
                        else objectToken.token = Guid.NewGuid().ToString();
                    }
                }
            });

            //�v���C���[���X�g�ɒǉ�
            PlayerList.Add(player, playerObj);

            return playerObj;
        }

        /// <summary>
        /// ���͌������Ȃ��I�u�W�F�N�g���X�|�[������
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public NetworkObject ObjectSpawned(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!Runner.IsServer) return null;

            //�I�u�W�F�N�g���X�|�[��������
            var networkObj = Runner.Spawn(prefab, position, rotation, null, (_, obj) =>
            {
                if (_configAsset.useHostMigration)
                {
                    //�I�u�W�F�N�g�g�[�N���̐ݒ�
                    if (obj.TryGetComponent<ObjectToken>(out var objectToken))
                        objectToken.token = Guid.NewGuid().ToString();
                }
            });

            return networkObj;
        }

        /// <summary>
        /// �v���C���[�̎Q��
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="player"></param>
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;

            if (_configAsset.useHostMigration)
            {
                //�����v���C���[���𔻕ʂ���
                int connectionToken = new Guid(runner.GetPlayerConnectionToken(player)).GetHashCode(); //�ڑ��g�[�N���̓ǂݏo��
                var playerList = FindObjectsOfType<ConnectionToken>();
                var isResumePlayer = playerList.FirstOrDefault(player => player.token == connectionToken);

                if (isResumePlayer == null) //�V�K�v���C���[
                {
                    //�V�����v���C���[���Q��
                    _newPlayerJoinedSubject.OnNext(player);
                }
                else //�����v���C���[
                {
                    //���͌����������v���C���[�ɕt�^
                    isResumePlayer.Object.AssignInputAuthority(player);

                    //�z�X�g�̃v���C���[�I�u�W�F�N�g�ɂ�HOST�t���O��t�^
                    if (isResumePlayer.TryGetComponent<NetworkObject>(out var playerObj))
                    {
                        if (playerObj.InputAuthority.PlayerId == runner.LocalPlayer.PlayerId
                            && playerObj.TryGetComponent<ObjectToken>(out var objectToken))
                        {
                            objectToken.token = "HOST";
                        }
                    }

                    //�v���C���[���X�g�ɒǉ�
                    PlayerList.Add(player, playerObj);
                }
            }
            else
            {
                //�V�����v���C���[���Q��
                _newPlayerJoinedSubject.OnNext(player);
            }
        }

        /// <summary>
        /// �v���C���[�̑ޏo
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
        /// �z�X�g�J�ڊJ�n���ɌĂяo�����
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
        /// �V�[���J�ڊ�����ɌĂяo�����
        /// </summary>
        /// <param name="runner"></param>
        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (!runner.IsServer || runner.IsResume) return;

            //���݂̃V�[���C���f�b�N�X�̃V�[���}�l�[�W���[���擾����
            if (_configAsset.sceneManagerTables.TryGetValue(SceneManager.GetActiveScene().buildIndex, out var sceneManagerPrefab))
            {
                //�V�[���}�l�[�W���[���X�|�[��������
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
