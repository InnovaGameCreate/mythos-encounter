using System.Collections;
using System.Collections.Generic;
using Scenes.Ingame.Manager;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Enemy.Trace
{

    public class AddTraceFeature : MonoBehaviour
    {
        TracePool _tracePool;
        GameObject _enemy;//TODO:敵が複数体だった場合の対応
        [SerializeField]
        private bool _debugAddTrace = false;
        [SerializeField]
        private TraceType[] _deugType;
        void Start()
        {
            _tracePool = GetComponent<TracePool>();
            IngameManager.Instance.OnIngame
                .Subscribe(_ =>
                {
                    _enemy = GameObject.FindWithTag("Enemy");
                    //TODO:敵から痕跡の情報をとって、生成する機能を追加する 
#if UNITY_EDITOR
                    if (_debugAddTrace)
                    {
                        foreach (var item in _deugType)
                        {
                            AddTrace(item);
                        }
                    }
#endif
                }).AddTo(this);
        }

        private void AddTrace(TraceType type)
        {
            var _instanceTrace = _tracePool.GetTraceObject(type);
            Instantiate(_instanceTrace, transform.position, Quaternion.identity, _enemy.transform);
        }
    }
}
