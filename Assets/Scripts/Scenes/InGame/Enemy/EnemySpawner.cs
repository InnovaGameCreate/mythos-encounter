using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] GameObject TestEnemy;


        // Start is called before the first frame update
        void Start()
        {
            //テストとしてここでEnemy制作を依頼している
            EnemySpawn(EnemyName.TestEnemy);
            EnemySpawn(EnemyName.TestEnemy,new Vector3(10,10,10));

        }

        // Update is called once per frame
        public void EnemySpawn(EnemyName enemeyName)
        {
            switch (enemeyName)
            {

                case EnemyName.TestEnemy:
                    GameObject.Instantiate(TestEnemy);
                    break;
                default:
                    Debug.LogError("このスクリプトに、すべての敵のプレハブが格納可能かか確認してください");
                    return;
            }
        }

        public void EnemySpawn(EnemyName enemeyName, Vector3 spownPosition)//位置を指定してスポーンさせたい場合
        {
            switch (enemeyName)
            {

                case EnemyName.TestEnemy:
                    GameObject.Instantiate(TestEnemy, spownPosition, Quaternion.identity);
                    break;
                default:
                    Debug.LogError("このスクリプトに、すべての敵のプレハブが格納可能かか確認してください");
                    return;
            }
        }

    }
}