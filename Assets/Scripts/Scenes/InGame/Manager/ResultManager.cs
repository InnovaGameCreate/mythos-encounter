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
        private CancellationTokenSource _token;
        private CancellationTokenSource _timerToken;
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
            _token = new CancellationTokenSource();
            _timerToken = new CancellationTokenSource();

            IngameManager.Instance.OnResult
                .Subscribe(_ =>
                {
                    _timerToken.Cancel();
                    _timerToken.Dispose();
                    _resultValue.time = eventManager.GetGameTime;
                    EnemyLevel();
                    _resultValue.getUnique = eventManager.GetUniqueItem;
                    _resultValue.firstContact = eventManager.GetContact;//TODO:����͓G�ɔ������ꂽ���ǂ��������Ă���B�i�ŏI�I�ɂ̓f�[�^�x�[�X����ߋ��ɑ����������Ƃ��邩�ǂ����Ŕ��f����j
                    _resultValue.totalMoney = Bonus();
                    _result.OnNext(_resultValue);
                }).AddTo(this);

        }
        /// <summary>
        /// �}�b�v���̐_�b�����ɍ��킹���{��
        /// TODO�F���x�G�L�����N�^�[�ɋ����̊T�O���ǉ����ꂽ�ꍇ�ɁA������n���Ă�������Ǝ�������B
        /// </summary>
        private void EnemyLevel()
        {
            _resultValue.level = 2;
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

        private void OnDestroy()
        {
            _token.Cancel();
            _token.Dispose();
        }
    }
}