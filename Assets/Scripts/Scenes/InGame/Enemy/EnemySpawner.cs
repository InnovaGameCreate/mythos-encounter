using Scenes.Ingame.Manager;
using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Manager;
using Scenes.Ingame.InGameSystem;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using Scenes.Ingame.Stage;




namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵キャラを作成する
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance;

        [Header("デバッグするかどうか")]
        [SerializeField] private bool _debugMode;
        [SerializeField][Tooltip("InGameManager無しで機能させるかどうか")] private bool _nonInGameManagerMode;
        [SerializeField][Tooltip("デバッグ時に作成する敵")] private EnemyName _enemyName;

        [Header("マップの設定")]
        [Header("スキャンするマップに関して")]
        [SerializeField]
        [Tooltip("自動で生成されるので挿入しない事")]
        private EnemyVisibilityMap _enemyVisibilityMap;
        [SerializeField]
        [Tooltip("各マス目の数")]
        private byte _x, _y, _z;
        [SerializeField]
        [Tooltip("マップのマス目の幅")]
        private float _range;
        [SerializeField]
        [Tooltip("最も視界の長い敵の視界の距離")]
        private float _maxVisiviilityRange;
        [SerializeField]
        [Tooltip("マップのマス目の最も左下のマス目の中心部")]
        private Vector3 _centerPosition;

        [Header("作成する敵のプレハブ一覧")]
        [SerializeField] private GameObject _testEnemy;
        [SerializeField] private GameObject _deepOnes;
        [SerializeField] private GameObject _spawnOfCthulhu;
        [SerializeField] private GameObject _MiGo;



        [Header("生成する際の設定")]
        [SerializeField] private Vector3 _enemySpawnPosition;


        private List<StageDoor> _doors = new List<StageDoor>();

        private CancellationTokenSource _cancellationTokenSource;

        // Start is called before the first frame update
        async void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            if (_nonInGameManagerMode)
            {
                InitialSpawn(_cancellationTokenSource.Token).Forget();
            }
            else
            {
                IngameManager.Instance.OnPlayerSpawnEvent.Subscribe(_ => InitialSpawn(_cancellationTokenSource.Token).Forget());//プレイヤースポーンはマップが完成してから行われる
            }

            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }



        private async UniTaskVoid InitialSpawn(CancellationToken token)
        {
            //ドアを入手
            _doors = new List<StageDoor>(FindObjectsOfType<StageDoor>());

            //全てのドアが動き終わったか確認する
            bool stop = false;
            while (!stop)
            {
                stop = true;
                //全てのドアが動き終わったか確認する
                for (int i = 0; i < _doors.Count; i++)
                {
                    if (_doors[i].ReturnIsAnimation)
                    {
                        stop = false;
                    }
                    if (!stop) await UniTask.Delay(100, cancellationToken: token);


                }
            }

            /*      なぜこれが上手くいかんのだ！？
            //全てのドアが動き終わったか確認する
            for (int i = 0; i < _doors.Count; i++)
            {
                Debug.Log("ここまで2");
                await UniTask.WaitWhile(() => !_doors[i].ReturnIsAnimation);
                Debug.Log("ここまで3");
            }
            */

            //全てのドアを閉める
            for (int i = 0; i < _doors.Count; i++)
            {
                _doors[i].ChangeDoorQuickOpen(false);
            }

            //全てのドアが動き終わったか確認する
            stop = false;
            while (!stop)
            {
                stop = true;
                //全てのドアが動き終わったか確認する
                for (int i = 0; i < _doors.Count; i++)
                {
                    if (_doors[i].ReturnIsAnimation)
                    {
                        stop = false;
                    }
                    if (!stop) await UniTask.Delay(100, cancellationToken: token);
                }
            }

            //マップをスキャン
            _enemyVisibilityMap = new EnemyVisibilityMap();
            _enemyVisibilityMap.debugMode = _debugMode;
            _enemyVisibilityMap.maxVisivilityRange = _maxVisiviilityRange;
            _enemyVisibilityMap.GridMake(_x, _y, _z, _range, _centerPosition);
            _enemyVisibilityMap.MapScan();


            //_doors[0].gameObject.transform.position = _doors[0].gameObject.transform.position + new Vector3(5,0,5);


            //コライダーの更新を待つ
            await UniTask.DelayFrame(2, PlayerLoopTiming.FixedUpdate, token);
            _enemyVisibilityMap.NeedOpenDoorScan();


            //全てのドアを開ける
            for (int i = 0; i < _doors.Count; i++)
            {
                _doors[i].ChangeDoorQuickOpen(true);
            }

            //全てのドアが動き終わったか確認する
            stop = false;
            while (!stop)
            {
                stop = true;
                //全てのドアが動き終わったか確認する
                for (int i = 0; i < _doors.Count; i++)
                {
                    if (_doors[i].ReturnIsAnimation)
                    {
                        stop = false;
                        await UniTask.Delay(100, cancellationToken: token);
                    }
                }
            }
            //コライダーの更新を待つ
            await UniTask.DelayFrame(2, PlayerLoopTiming.FixedUpdate, token);
            _enemyVisibilityMap.NeedCloseDoorScan();


            //全てのドアを初期状態にする
            for (int i = 0; i < _doors.Count; i++)
            {
                _doors[i].ChangeDoorInitial();
            }

            if (_nonInGameManagerMode)
            {
                EnemySpawn(EnemyName.TestEnemy, new Vector3(-10, _centerPosition.y + 3, -10));
            }
            else
            {
                //ここでEnemy制作
                EnemySpawn(_enemyName, _enemySpawnPosition);
                //敵の沸きが完了したことを知らせる
                IngameManager.Instance.SetReady(ReadyEnum.EnemyReady);
            }
        }



        public void EnemySpawn(EnemyName enemeyName, Vector3 spownPosition)//位置を指定してスポーンさせたい場合
        {
            GameObject createEnemy;
            EnemyStatus createEnemyStatus;
            EnemyVisibilityMap createEnemyVisiviityMap = _enemyVisibilityMap.DeepCopy();
            switch (enemeyName)
            {

                case EnemyName.TestEnemy:
                    createEnemy = GameObject.Instantiate(_testEnemy, spownPosition, Quaternion.identity);
                    if (_debugMode) Debug.Log("エネミーは制作されました");
                    break;
                case EnemyName.DeepOnes:
                    createEnemy = GameObject.Instantiate(_deepOnes, spownPosition, Quaternion.identity);
                    if (_debugMode) Debug.Log("エネミーは制作されました");
                    break;
                case EnemyName.SpawnOfCthulhu:
                    createEnemy = GameObject.Instantiate(_spawnOfCthulhu, spownPosition, Quaternion.identity);
                    if (_debugMode) Debug.Log("エネミーは制作されました");
                    break;
                case EnemyName.MiGo:
                    createEnemy = GameObject.Instantiate(_MiGo, spownPosition, Quaternion.identity);
                    if (_debugMode) Debug.Log("エネミーは制作されました");
                    break;
                default:
                    Debug.LogError("このスクリプトに、すべての敵のプレハブが格納可能かを確認してください");
                    return;
            }
            if (createEnemy.TryGetComponent<EnemyStatus>(out createEnemyStatus))
            {
                if (_debugMode) Debug.Log("作成した敵にはEnemyStatusクラスがあります");
                createEnemyVisiviityMap.DontApproachPlayer();
                createEnemyStatus.Init(createEnemyVisiviityMap);

            }

        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

    }
}