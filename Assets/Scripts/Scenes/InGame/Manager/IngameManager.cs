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
        private Subject<Unit> _finishStageGenerateEvent = new Subject<Unit>();
        public IObservable<Unit> OnInitial { get { return _initialEvent; } }
        public IObservable<Unit> OnIngame { get { return _ingameEvent; } }
        public IObservable<Unit> OnOpenEscapePointEvent { get { return _openEscapePointEvent; } }
        public IObservable<Unit> OnResult { get { return _resultEvent; } }
        public IObservable<Unit> OnOutgame { get { return _outgameEvent; } }
        public IObservable<Unit> OnFinishStageGenerateEvent { get { return _finishStageGenerateEvent; } }

        public IngameState CurrentState { get => _currentState; }

        [SerializeField]
        private int _escapeItemCount;//脱出までに必要な脱出アイテムの数
        private int _getEscapeItemCount;//現在取得している脱出アイテムの数

        void Awake()
        {
            Instance = this;
            _ingameReady.Initialize();
            OnInitial.Subscribe(_ => InitialState().Forget()).AddTo(this);
            OnIngame.Subscribe(_ => InGameState()).AddTo(this);
            OnResult.Subscribe(_ => ResultState()).AddTo(this);
            OnOutgame.Subscribe(_ => OutGameState()).AddTo(this);
        }
        private async void Start()
        {
            await Task.Delay(500);
            _initialEvent.OnNext(default);
            _ingameEvent.OnNext(default);
        }
        private async Cysharp.Threading.Tasks.UniTaskVoid InitialState()
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
            if(ready == ReadyEnum.StageReady)
            {
                _finishStageGenerateEvent.OnNext(default);
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
            Debug.Log("脱出アイテムを獲得しました");
            _getEscapeItemCount++;
            if(_getEscapeItemCount >= _escapeItemCount)
            {
                _openEscapePointEvent.OnNext(default);
            }
        }
    }
}