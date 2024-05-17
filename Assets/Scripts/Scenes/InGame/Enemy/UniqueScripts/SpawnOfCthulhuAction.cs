using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class SpawnOfCthulhuAction : EnemyUniqueAction
    {
        private EnemySpawner _enemySpawner;

        protected override void Start()
        {
            _enemySpawner = GameObject.Find("EnemySpowner").GetComponent<EnemySpawner>();
        }

        protected override void Action()
        {
            _enemySpawner.EnemySpawn(EnemyName.DeepOnes, this.transform.position);
        }
    }
}
