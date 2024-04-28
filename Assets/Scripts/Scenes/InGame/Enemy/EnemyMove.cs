using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵キャラクターの移動を管理する
    /// </summary>
    public class EnemyMove : MonoBehaviour
    {
        public bool endMove;
        private NavMeshAgent _myAgent;
        // Start is called before the first frame update
        void Start()
        {
            _myAgent = GetComponent<NavMeshAgent>();
            if (_myAgent == null) Debug.LogError("NavMeshAgentが認識できません");
            _myAgent.destination = this.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Magnitude(this.transform.position - _myAgent.destination) < 1f) { endMove = true; } else { endMove = false; }
        }

        public void SetMovePosition(Vector3 targetPosition) {
            _myAgent.destination = targetPosition;
        }
    }
}