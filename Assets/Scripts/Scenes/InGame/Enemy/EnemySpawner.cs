using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// �G�L�������쐬����
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("�f�o�b�O���邩�ǂ���")]
        [SerializeField] private bool _debugMode;
        [SerializeField][Tooltip("InGameManager�����ŋ@�\�����邩�ǂ���")] private bool _nonInGameManagerMode;
        [SerializeField][Tooltip("�f�o�b�O���ɍ쐬����G")]private EnemyName _enemyName;

        [Header("�X�L��������}�b�v�Ɋւ���")]
        [SerializeField]
        private EnemyVisibilityMap _enemyVisibilityMap;
        [SerializeField]
        private byte _x, _z;
        [SerializeField]
        private float _range;
        [SerializeField]
        private float _maxVisiviilityRange;
        [SerializeField]
        private Vector3 _centerPosition;

        [Header("�쐬����G�̃v���n�u�ꗗ")]
        [SerializeField] private GameObject _testEnemy;
        [SerializeField] private GameObject _deepOnes;
        [SerializeField] private GameObject _spawnOfCthulhu;
        [SerializeField] private GameObject _MiGo;


        // Start is called before the first frame update
        void Start()
        {
            //�}�b�v���X�L����
            _enemyVisibilityMap = new EnemyVisibilityMap();
            _enemyVisibilityMap.debugMode = _debugMode;
            _enemyVisibilityMap.maxVisivilityRange = _maxVisiviilityRange;
            _enemyVisibilityMap.GridMake(_x, _z, _range, _centerPosition);
            _enemyVisibilityMap.MapScan();

            //�e�X�g�Ƃ��Ă�����Enemy������˗����Ă���
            EnemySpawn(_enemyName, new Vector3(-10, _centerPosition.y+3, -10));            
        }



        public void EnemySpawn(EnemyName enemeyName, Vector3 spownPosition)//�ʒu���w�肵�ăX�|�[�����������ꍇ
        {
            GameObject createEnemy;
            EnemySearch createEnemySearch;
            EnemyStatus createEnemyStatus;
            switch (enemeyName)
            {

                case EnemyName.TestEnemy:
                    createEnemy = GameObject.Instantiate(_testEnemy, spownPosition, Quaternion.identity);
                    if (_debugMode) Debug.Log("�G�l�~�[�͐��삳��܂���");
                    break;
                case EnemyName.DeepOnes:
                    createEnemy = GameObject.Instantiate(_deepOnes, spownPosition, Quaternion.identity);
                    if (_debugMode) Debug.Log("�G�l�~�[�͐��삳��܂���");
                    break;
                case EnemyName.SpawnOfCthulhu:
                    createEnemy = GameObject.Instantiate(_spawnOfCthulhu, spownPosition, Quaternion.identity);
                    if (_debugMode) Debug.Log("�G�l�~�[�͐��삳��܂���");
                    break;
                case EnemyName.MiGo:
                    createEnemy = GameObject.Instantiate(_MiGo, spownPosition, Quaternion.identity);
                    if (_debugMode) Debug.Log("�G�l�~�[�͐��삳��܂���");
                    break;
                default:
                    Debug.LogError("���̃X�N���v�g�ɁA���ׂĂ̓G�̃v���n�u���i�[�\�����m�F���Ă�������");
                    return;
            }
            if (createEnemy.TryGetComponent<EnemyStatus>(out createEnemyStatus))
            {
                if (_debugMode) Debug.Log("�쐬�����G�ɂ�EnemyStatus�N���X������܂�");
                createEnemyStatus.Init(_enemyVisibilityMap.DeepCopy());
            }

        }

    }
}