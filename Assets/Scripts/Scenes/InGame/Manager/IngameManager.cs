using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

namespace Scenes.Ingame.Manager
{
    public class IngameManager : MonoBehaviour
    {
        private IngameState _currentState = IngameState.Outgame;
        public static IngameManager Instance;
        private IngameReady _ingameReady;

        private Subject<Unit> _initialEvent = new Subject<Unit>();
        private Subject<Unit> _ingameEvent = new Subject<Unit>();
        private Subject<Unit> _resultEvent = new Subject<Unit>();
        private Subject<Unit> _outgameEvent = new Subject<Unit>();
        public IObservable<Unit> OnInitial { get { return _initialEvent; } }
        public IObservable<Unit> OnIngame { get { return _ingameEvent; } }
        public IObservable<Unit> OnResult { get { return _resultEvent; } }
        public IObservable<Unit> OnOutgame { get { return _outgameEvent; } }
        void Awake()
        {
            Instance = this;
            _ingameReady.Initialize();
            OnInitial.Subscribe(_ => InitialState().Forget()).AddTo(this);
            OnIngame.Subscribe(_ => InGameState()).AddTo(this);
            OnResult.Subscribe(_ => ResultState()).AddTo(this);
            OnOutgame.Subscribe(_ => OutGameState()).AddTo(this);
            _ingameEvent.OnNext(default);
        }

        private async UniTaskVoid InitialState()
        {
            Debug.Log("Current State is Initial!");
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
            _ingameReady.SetReady(ready);
        }
        public void Escape()
        {
            _resultEvent.OnNext(default);
        }
    }
}