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
            //�e�X�g�Ƃ��Ă�����Enemy������˗����Ă���
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
                    Debug.LogError("���̃X�N���v�g�ɁA���ׂĂ̓G�̃v���n�u���i�[�\�����m�F���Ă�������");
                    return;
            }
        }

        public void EnemySpawn(EnemyName enemeyName, Vector3 spownPosition)//�ʒu���w�肵�ăX�|�[�����������ꍇ
        {
            switch (enemeyName)
            {

                case EnemyName.TestEnemy:
                    GameObject.Instantiate(TestEnemy, spownPosition, Quaternion.identity);
                    break;
                default:
                    Debug.LogError("���̃X�N���v�g�ɁA���ׂĂ̓G�̃v���n�u���i�[�\�����m�F���Ă�������");
                    return;
            }
        }

    }
}