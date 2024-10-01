using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Scenes.Ingame.InGameSystem;
using System;
namespace Scenes.Ingame.Manager
{
    public class ResultManager : MonoBehaviour
    {
        public static ResultManager Instance;
        private ResultValue _resultValue;
        private Subject<ResultValue> _result = new Subject<ResultValue>();
        public IObservable<ResultValue> OnResultValue { get { return _result; } }
        EventManager eventManager;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            eventManager = EventManager.Instance;
            _resultValue = new ResultValue();

            IngameManager.Instance.OnResult
                .Subscribe(_ =>
                {
                    _resultValue.time = eventManager.GetGameTime;
                    _resultValue.level = eventManager.EnemyLevel();
                    _resultValue.getUnique = eventManager.GetUniqueItem;
                    _resultValue.firstContact = eventManager.GetContact;
                    _resultValue.totalMoney = Bonus();
                    _result.OnNext(_resultValue);
                }).AddTo(this);

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
    }
}