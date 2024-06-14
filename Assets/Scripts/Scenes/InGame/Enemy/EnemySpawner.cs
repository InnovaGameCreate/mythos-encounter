using Scenes.Ingame.Manager;
using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Manager;
using Scenes.Ingame.InGameSystem;



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
        [SerializeField][Tooltip("デバッグ時に作成する敵")]private EnemyName _enemyName;

        [Header("マップの設定")]
        [Header("スキャンするマップに関して")]
        [SerializeField]
        [Tooltip("自動で生成されるので挿入しない事")]
        private EnemyVisibilityMap _enemyVisibilityMap;
        [SerializeField]
        [Tooltip("各マス目の数")]
        private byte _x, _z;
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

        // Start is called before the first frame update
        void Start()
        {
            if (_nonInGameManagerMode)
            {
                //マップをスキャン
                _enemyVisibilityMap = new EnemyVisibilityMap();
                _enemyVisibilityMap.debugMode = _debugMode;
                _enemyVisibilityMap.maxVisivilityRange = _maxVisiviilityRange;
                _enemyVisibilityMap.GridMake(_x, _z, _range, _centerPosition);
                _enemyVisibilityMap.MapScan();

                //テストとしてここでEnemy制作を依頼している
                EnemySpawn(_enemyName, new Vector3(-10, _centerPosition.y + 3, -10));

            }
            else {
                IngameManager.Instance.OnPlayerSpawnEvent.Subscribe(_ => InitialSpawn());
            }

            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);

        }

        public void InitialSpawn() {

            //マップをスキャン
            _enemyVisibilityMap = new EnemyVisibilityMap();
            _enemyVisibilityMap.debugMode = _debugMode;
            _enemyVisibilityMap.maxVisivilityRange = _maxVisiviilityRange;
            _enemyVisibilityMap.GridMake(_x, _z, _range, _centerPosition);
            _enemyVisibilityMap.MapScan();

            //ここでEnemy制作
            EnemySpawn(_enemyName, _enemySpawnPosition);
            //敵の沸きが完了したことを知らせる
            IngameManager.Instance.SetReady(ReadyEnum.EnemyReady);

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

    }
}