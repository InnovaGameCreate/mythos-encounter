using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("デバッグするかどうか")]
        [SerializeField] private bool _DebugMode;

        [Header("スキャンするマップに関して")]
        [SerializeField]
        private EnemyVisibilityMap _enemyVisibilityMap;
        [SerializeField]
        private byte _x, _z;
        [SerializeField]
        private float _range;
        [SerializeField]
        private Vector3 _centerPosition;

        [Header("作成する敵のプレハブ一覧")]
        [SerializeField] private GameObject _testEnemy;
        

        // Start is called before the first frame update
        void Start()
        {
            //マップをスキャン
            _enemyVisibilityMap = new EnemyVisibilityMap();
            _enemyVisibilityMap.GridMake(_x, _z, _range, _centerPosition);
            _enemyVisibilityMap.MapScan();

            //テストとしてここでEnemy制作を依頼している
            //EnemySpawn(EnemyName.TestEnemy);
            EnemySpawn(EnemyName.TestEnemy, new Vector3(10, 10, 10));            

        }

        /*
        public void EnemySpawn(EnemyName enemeyName)//ポリモーフィズムには死んでもらいました
        {
            switch (enemeyName)
            {

                case EnemyName.TestEnemy:
                    GameObject.Instantiate(_testEnemy);
                    if (_DebugMode) Debug.Log("エネミーは制作されました");
                    break;
                default:
                    Debug.LogError("このスクリプトに、すべての敵のプレハブが格納可能かか確認してください");
                    return;
            }
        }
        */

        public void EnemySpawn(EnemyName enemeyName, Vector3 spownPosition)//位置を指定してスポーンさせたい場合
        {
            GameObject createEnemy;
            EnemySearch createEnemySearch;
            switch (enemeyName)
            {

                case EnemyName.TestEnemy:
                    createEnemy = GameObject.Instantiate(_testEnemy, spownPosition, Quaternion.identity);
                    if (_DebugMode) Debug.Log("エネミーは制作されました");
                    break;
                default:
                    Debug.LogError("このスクリプトに、すべての敵のプレハブが格納可能かを確認してください");
                    return;
            }

            createEnemySearch = createEnemy.GetComponent<EnemySearch>();
        }

    }
}