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
    /// �G�L�������쐬����
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance;

        [Header("�f�o�b�O���邩�ǂ���")]
        [SerializeField] private bool _debugMode;
        [SerializeField][Tooltip("InGameManager�����ŋ@�\�����邩�ǂ���")] private bool _nonInGameManagerMode;
        [SerializeField][Tooltip("�f�o�b�O���ɍ쐬����G")]private EnemyName _enemyName;

        [Header("�}�b�v�̐ݒ�")]
        [Header("�X�L��������}�b�v�Ɋւ���")]
        [SerializeField]
        [Tooltip("�����Ő��������̂ő}�����Ȃ���")]
        private EnemyVisibilityMap _enemyVisibilityMap;
        [SerializeField]
        [Tooltip("�e�}�X�ڂ̐�")]
        private byte _x, _z;
        [SerializeField]
        [Tooltip("�}�b�v�̃}�X�ڂ̕�")]
        private float _range;
        [SerializeField]
        [Tooltip("�ł����E�̒����G�̎��E�̋���")]
        private float _maxVisiviilityRange;
        [SerializeField]
        [Tooltip("�}�b�v�̃}�X�ڂ̍ł������̃}�X�ڂ̒��S��")]
        private Vector3 _centerPosition;

        [Header("�쐬����G�̃v���n�u�ꗗ")]
        [SerializeField] private GameObject _testEnemy;
        [SerializeField] private GameObject _deepOnes;
        [SerializeField] private GameObject _spawnOfCthulhu;
        [SerializeField] private GameObject _MiGo;



        [Header("��������ۂ̐ݒ�")]
        [SerializeField] private Vector3 _enemySpawnPosition;

        // Start is called before the first frame update
        void Start()
        {
            if (_nonInGameManagerMode)
            {
                //�}�b�v���X�L����
                _enemyVisibilityMap = new EnemyVisibilityMap();
                _enemyVisibilityMap.debugMode = _debugMode;
                _enemyVisibilityMap.maxVisivilityRange = _maxVisiviilityRange;
                _enemyVisibilityMap.GridMake(_x, _z, _range, _centerPosition);
                _enemyVisibilityMap.MapScan();

                //�e�X�g�Ƃ��Ă�����Enemy������˗����Ă���
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

            //�}�b�v���X�L����
            _enemyVisibilityMap = new EnemyVisibilityMap();
            _enemyVisibilityMap.debugMode = _debugMode;
            _enemyVisibilityMap.maxVisivilityRange = _maxVisiviilityRange;
            _enemyVisibilityMap.GridMake(_x, _z, _range, _centerPosition);
            _enemyVisibilityMap.MapScan();

            //������Enemy����
            EnemySpawn(_enemyName, _enemySpawnPosition);
            //�G�̕����������������Ƃ�m�点��
            IngameManager.Instance.SetReady(ReadyEnum.EnemyReady);

        }



        public void EnemySpawn(EnemyName enemeyName, Vector3 spownPosition)//�ʒu���w�肵�ăX�|�[�����������ꍇ
        {
            GameObject createEnemy;
            EnemyStatus createEnemyStatus;
            EnemyVisibilityMap createEnemyVisiviityMap = _enemyVisibilityMap.DeepCopy();
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
                createEnemyVisiviityMap.DontApproachPlayer();
                createEnemyStatus.Init(createEnemyVisiviityMap);

            }

        }

    }
}