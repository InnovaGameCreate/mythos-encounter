using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

namespace Network
{
    public class HostMigrationHandler : MonoBehaviour
    {
        private List<string> _resumeTokens = new List<string>(); //�ߋ��Z�b�V�����̃I�u�W�F�N�g���X�g���i�[����

        /// <summary>
        /// Runner���ċN��������
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="hostMigrationToken"></param>
        public async void RebootRunner(NetworkRunner runnerPrefab, NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            //���g�̐ڑ��g�[�N�����擾
            var connectionTokenBytes = runner.GetPlayerConnectionToken(runner.LocalPlayer);

            //�I�u�W�F�N�g�g�[�N�����X�g���擾
            var tokens = FindObjectsOfType<ObjectToken>().Select(x => x.token);
            _resumeTokens = new List<string>(tokens);

            //Runner�̒�~
            await runner.Shutdown(true, ShutdownReason.HostMigration);
            RunnerManager.Runner = null;
            RunnerManager.Instance = null;

            //Runner�̍ċN��
            runner = Instantiate(runnerPrefab);
            runner.ProvideInput = true;

            //Runner��StartGame���\�b�h�̈���
            var args = new StartGameArgs
            {
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
                HostMigrationToken = hostMigrationToken,
                HostMigrationResume = HostMigrationResume,
                ConnectionToken = connectionTokenBytes,
            };

            //�Z�b�V�������J�n����
            await RunnerManager.Instance.JoinSession(args);

            //�n���h���̏I��
            Destroy(this.gameObject);
        }

        /// <summary>
        /// �V�Z�b�V�����Ɉڍs�����Ƃ��A�z�X�g�����Ăяo�����
        /// </summary>
        /// <param name="runner"></param>
        private void HostMigrationResume(NetworkRunner runner)
        {
            //�ߋ��Z�b�V�����̃l�b�g���[�N�I�u�W�F�N�g�𕜌�
            foreach (var resumeObj in runner.GetResumeSnapshotNetworkObjects())
            {
                //ObjectTokenCs����g�[�N����Transform�̏��𔲂��o��
                var objectTokenCs = resumeObj.GetComponent<ObjectToken>();
                string objectToken = objectTokenCs.token;
                Vector3 position = objectTokenCs.position;
                Quaternion rotation = objectTokenCs.rotation;

                //�z�X�g���L�̃l�b�g���[�N�I�u�W�F�N�g�ł���΁A�������Ȃ�
                if (objectToken == "HOST") continue;

                //�g�[�N�����ߋ��Z�b�V�����̃g�[�N�����X�g�ɂ���΃X�|�[��������A
                if (_resumeTokens.Exists(x => x == objectToken))
                {
                    runner.Spawn(resumeObj, position, rotation, null, (_, obj) =>
                    {
                        obj.CopyStateFrom(resumeObj);
                    });
                }
            }
        }
    }
}
