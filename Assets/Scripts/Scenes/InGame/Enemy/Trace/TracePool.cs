using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Scenes.Ingame.Enemy.Trace
{
    public class TracePool : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _traceList;

        public GameObject GetTraceObject(TraceType type)
        {
            return _traceList.FirstOrDefault(trace => trace.name.Equals(type.ToString()));
        }
    }
}