using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy.Trace
{

    public class AddTraceFeature : MonoBehaviour
    {
        TracePool _tracePool;
        GameObject _enemy;//TODO:敵が複数体だった場合の対応
        void Start()
        {
            _tracePool = GetComponent<TracePool>();
            _enemy = GameObject.FindWithTag("Enemy");
        }

        private void AddTrace(TraceType type)
        {
            var _instanceTrace = _tracePool.GetTraceObject(type);
            Instantiate(_instanceTrace, transform.position, Quaternion.identity, _enemy.transform);
;        }
    }
}
