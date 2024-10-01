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
    /// ?v???C???[?????????????????????????B?????L???A???V??????????????????????????
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        private int _gameTime;//?v???C????
        private bool _getUniqueItem = false;//???j?[?N?A?C?e?????????L??
        private int _chaseCount = 0;
        CancellationTokenSource _source = new CancellationTokenSource();
        private EnemyStatus _enemyStatus;
        public int GetGameTime { get => _gameTime; }
        public bool GetContact { get
            {
                if(_enemyStatus != null)
                {
                    return PlayerInformationFacade.Instance.IsFarstContactEnemy(_enemyStatus.EnemyId);
                }
                else
                {
                    return false;
                }
            } }
        public bool GetUniqueItem { get => _getUniqueItem; }
        public static EventManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            IngameManager.Instance.OnIngame
                .Subscribe(_ => Init());
        }

        private void Init()
        {
            CancellationToken token = _source.Token;
            IngameManager.Instance.OnIngame
                .Subscribe(_ =>
                {
                    GameTime(token).Forget();
                }).AddTo(this);

            _enemyStatus = FindObjectOfType<EnemyStatus>();
            _enemyStatus.OnEnemyStateChange
                .Where(state => state == EnemyState.Chase)
                .Subscribe(_ =>
                {
                    _chaseCount++;
                }).AddTo(this);
        }

        public int EnemyLevel()
        {
            return 1 + (_enemyStatus.EnemyId / 3);
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