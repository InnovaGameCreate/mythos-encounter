using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Tooltip:�����Ɋe��G�̖��O�Ƃ���ɑΉ�����v���n�u��ǉ����Ă�������]
        [SerializeField]Dictionary<EnemyName, GameObject> dic = new Dictionary<EnemyName, GameObject>();


        // Start is called before the first frame update
        void Start()
        {
            //�e�X�g�Ƃ��Ă�����Enemy������˗����Ă���
            EnemySpawn(EnemyName.TestEnemy);
        }

        // Update is called once per frame
        public void EnemySpawn(EnemyName nemeyName) {
    
        }
    }
}