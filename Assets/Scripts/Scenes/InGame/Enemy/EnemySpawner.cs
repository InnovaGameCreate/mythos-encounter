using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Tooltip:ここに各種敵の名前とそれに対応するプレハブを追加してください]
        [SerializeField]Dictionary<EnemyName, GameObject> dic = new Dictionary<EnemyName, GameObject>();


        // Start is called before the first frame update
        void Start()
        {
            //テストとしてここでEnemy制作を依頼している
            EnemySpawn(EnemyName.TestEnemy);
        }

        // Update is called once per frame
        public void EnemySpawn(EnemyName nemeyName) {
    
        }
    }
}