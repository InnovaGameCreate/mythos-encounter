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
        private List<string> _resumeTokens = new List<string>(); //過去セッションのオブジェクトリストを格納する

        /// <summary>
        /// Runnerを再起動させる
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="hostMigrationToken"></param>
        public async void RebootRunner(NetworkRunner runnerPrefab, NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            //自身の接続トークンを取得
            var connectionTokenBytes = runner.GetPlayerConnectionToken(runner.LocalPlayer);

            //オブジェクトトークンリストを取得
            var tokens = FindObjectsOfType<ObjectToken>().Select(x => x.token);
            _resumeTokens = new List<string>(tokens);

            //Runnerの停止
            await runner.Shutdown(true, ShutdownReason.HostMigration);
            RunnerManager.Runner = null;
            RunnerManager.Instance = null;

            //Runnerの再起動
            runner = Instantiate(runnerPrefab);
            runner.ProvideInput = true;

            //RunnerのStartGameメソッドの引数
            var args = new StartGameArgs
            {
                Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
                SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
                HostMigrationToken = hostMigrationToken,
                HostMigrationResume = HostMigrationResume,
                ConnectionToken = connectionTokenBytes,
            };

            //セッションを開始する
            await RunnerManager.Instance.JoinSession(args);

            //ハンドラの終了
            Destroy(this.gameObject);
        }

        /// <summary>
        /// 新セッションに移行したとき、ホストだけ呼び出される
        /// </summary>
        /// <param name="runner"></param>
        private void HostMigrationResume(NetworkRunner runner)
        {
            //過去セッションのネットワークオブジェクトを復元
            foreach (var resumeObj in runner.GetResumeSnapshotNetworkObjects())
            {
                //ObjectTokenCsからトークンやTransformの情報を抜き出す
                var objectTokenCs = resumeObj.GetComponent<ObjectToken>();
                string objectToken = objectTokenCs.token;
                Vector3 position = objectTokenCs.position;
                Quaternion rotation = objectTokenCs.rotation;

                //ホスト所有のネットワークオブジェクトであれば、復元しない
                if (objectToken == "HOST") continue;

                //トークンが過去セッションのトークンリストにあればスポーンさせる、
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
