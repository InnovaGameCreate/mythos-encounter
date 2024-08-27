using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Scenes.Ingame.Enemy;
using Data;

namespace Scenes.Ingame.Manager
{
    /// <summary>
    /// �v���C���[�̎��т�����̉�������̒B���̗L���A��V�Ɋւ��邱�ƂȂǂ��Ǘ�����
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        private int _gameTime;//�v���C����
        private bool _getUniqueItem = false;//���j�[�N�A�C�e���擾�̗L��
        private float _playerMoveDistance = 0;
        private int _chaseCount = 0;
        CancellationTokenSource _source = new CancellationTokenSource();

        public int GetGameTime { get => _gameTime; }
        public bool GetContact { get => PlayerInformationFacade.Instance.IsFarstContactEnemy(0); }//TODO�ォ��G�L�����N�^�[��ID���擾����
        public bool GetUniqueItem { get => _getUniqueItem; }
        public static EventManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            CancellationToken token = _source.Token;
            IngameManager.Instance.OnIngame
                .Subscribe(_ =>
                {
                    GameTime(token).Forget();
                }).AddTo(this);
        }

        private void Init()
        {
            EnemyStatus enemyStatus = FindObjectOfType<EnemyStatus>();
            enemyStatus.OnEnemyStateChange
                .Where(state => state == EnemyState.Chase)
                .Subscribe(_ =>
                {
                    _chaseCount++;
                }).AddTo(this);
        }
        void Update()
        {

        }
        async UniTaskVoid GameTime(CancellationToken token)
        {
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
                _gameTime++;
            }
        }
        public void UniqueItemGet()
        {
            _getUniqueItem = true;
        }
        private void OnDestroy()
        {
            _source.Cancel();
            _source.Dispose();
        }
    }
}