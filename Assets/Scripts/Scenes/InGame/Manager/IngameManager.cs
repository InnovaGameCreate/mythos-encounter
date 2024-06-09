using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using Scenes.Ingame.InGameSystem;

namespace Scenes.Ingame.Manager
{
    public class IngameManager : MonoBehaviour
    {
        private IngameState _currentState = IngameState.Outgame;
        public static IngameManager Instance;
        private IngameReady _ingameReady;

        private Subject<Unit> _initialEvent = new Subject<Unit>();
        private Subject<Unit> _ingameEvent = new Subject<Unit>();
        private Subject<Unit> _openEscapePointEvent = new Subject<Unit>();
        private Subject<Unit> _resultEvent = new Subject<Unit>();
        private Subject<Unit> _outgameEvent = new Subject<Unit>();
        private Subject<Unit> _stageGenerateEvent = new Subject<Unit>();
        private Subject<Unit> _playerSpawnEvent = new Subject<Unit>();
        public IObservable<Unit> OnInitial { get { return _initialEvent; } }
        public IObservable<Unit> OnIngame { get { return _ingameEvent; } }
        public IObservable<Unit> OnOpenEscapePointEvent { get { return _openEscapePointEvent; } }
        public IObservable<Unit> OnResult { get { return _resultEvent; } }
        public IObservable<Unit> OnOutgame { get { return _outgameEvent; } }
        public IObservable<Unit> OnStageGenerateEvent { get { return _stageGenerateEvent; } }
        public IObservable<Unit> OnPlayerSpawnEvent { get { return _playerSpawnEvent; } }

        public IngameState CurrentState { get => _currentState; }

        [SerializeField]
        private int _escapeItemCount;//脱出までに必要な脱出アイテムの数
        private ReactiveProperty<int> _getEscapeItemCount = new ReactiveProperty<int>();
        public IObservable<int> OnEscapeCount => _getEscapeItemCount; //現在取得している脱出アイテムの数

        public int GetEscapeItemCount { get => _escapeItemCount; }

        void Awake()
        {
            Instance = this;
            OnInitial.Subscribe(_ => InitialState().Forget()).AddTo(this);
            OnIngame.Subscribe(_ => InGameState()).AddTo(this);
            OnResult.Subscribe(_ => ResultState()).AddTo(this);
            OnOutgame.Subscribe(_ => OutGameState()).AddTo(this);
        }
        private async void Start()
        {
            await Task.Delay(500);
            _initialEvent.OnNext(default);
            Debug.Log("Current State is Initial!");
        }
        private async UniTaskVoid InitialState()
        {
            _currentState = IngameState.Initial;
            await UniTask.WaitUntil(_ingameReady.Ready);
            _ingameEvent.OnNext(default);
        }

        private void InGameState()
        {
            Debug.Log("Current State is InGame!");
            _currentState = IngameState.Ingame;
        }

        private void ResultState()
        {
            Debug.Log("Current State is Result!");
            _currentState = IngameState.Result;
        }
        private void OutGameState()
        {
            Debug.Log("Current State is OutGame!");
            _currentState = IngameState.Outgame;
        }

        public void SetReady(ReadyEnum ready)
        {
            Debug.Log($"SetReady.value = {ready}");
            switch (ready)
            {
                case ReadyEnum.StageReady:
                    _stageGenerateEvent.OnNext(default);
                    break;
                case ReadyEnum.PlayerReady:
                    _playerSpawnEvent.OnNext(default);
                    break;
                case ReadyEnum.EnemyReady:
                    break;
                default:
                    break;
            }
            _ingameReady.SetReady(ready);
        }

        //プレイヤーが脱出した際の処理
        public void Escape()
        {
            Debug.Log("脱出しました");
            _resultEvent.OnNext(default);
        }

        //プレイヤーが脱出アイテムを入手した際の処理
        public void GetEscapeItem()
        {
            _getEscapeItemCount.Value++;
            Debug.Log($"脱出アイテムを獲得しました。現在の個数は{_getEscapeItemCount.Value}個、必要な個数は{_escapeItemCount}個、解放状態は{_getEscapeItemCount.Value >= _escapeItemCount}");
            if (_getEscapeItemCount.Value >= _escapeItemCount)
            {
                _openEscapePointEvent.OnNext(default);
            }
        }
    }
}