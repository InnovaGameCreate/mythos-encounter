using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using Scenes.Ingame.InGameSystem;
using System;
namespace Scenes.Ingame.Manager
{
    public class ResultManager : MonoBehaviour
    {
        private int _ingameTimer = 0;
        private CancellationTokenSource _token;
        private CancellationTokenSource _timerToken;
        public static ResultManager Instance;
        private ResultValue _resultValue;
        private Subject<ResultValue> _result = new Subject<ResultValue>();
        public IObservable<ResultValue> OnResultValue { get { return _result; } }
        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            _resultValue = new ResultValue();
            _token = new CancellationTokenSource();
            _timerToken = new CancellationTokenSource();

            IngameManager.Instance.OnIngame
                .Subscribe(_ =>
                {
                    Timer(_timerToken.Token).Forget();
                }).AddTo(this);
            IngameManager.Instance.OnResult
                .Subscribe(_ =>
                {
                    _timerToken.Cancel();
                    _timerToken.Dispose();
                    Time();
                    EnemyLevel();
                    UniqueItemBonus();
                    FirstContactBonus();
                    _resultValue.totalMoney = Bonus();
                    _result.OnNext(_resultValue);
                }).AddTo(this);

        }

        private void Time()
        {
            _resultValue.time = _ingameTimer;
        }
        /// <summary>
        /// マップ内の神話生物に合わせた倍率
        /// TODO：今度敵キャラクターに強さの概念が追加された場合に、引数を渡してしっかりと実装する。
        /// </summary>
        private void EnemyLevel()
        {
            _resultValue.level = 2;
        }
        //ユニークアイテムの獲得
        private void UniqueItemBonus()
        {
            _resultValue.getUnique = false;
        }
        /// <summary>
        /// 遭遇状況(初回のみ加算)
        /// TODO：今後データベースが実装されたら書き直す。
        /// </summary>
        private void FirstContactBonus()
        {
            _resultValue.firstContact = true;
        }
        private int Bonus()
        {
            int money = 100;
            money += (20 - _resultValue.time / 60) * 5 > 0 ? (20 - _resultValue.time / 60) * 5 : 0;
            money += 20 * _resultValue.level;
            money += _resultValue.getUnique ? 50 : 0;
            money += _resultValue.firstContact ? 100 : 0;
            return money;

        }
        private async UniTaskVoid Timer(CancellationToken token)
        {
            while (true)
            {
                await UniTask.Delay(1000, cancellationToken: token);
                _ingameTimer++;
            }
        }

        private void OnDestroy()
        {
            _token.Cancel();
            _token.Dispose();
        }
    }
}