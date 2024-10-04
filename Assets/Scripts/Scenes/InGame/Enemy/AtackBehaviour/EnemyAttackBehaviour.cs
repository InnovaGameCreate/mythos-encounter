using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Fusion;

namespace Scenes.Ingame.Enemy
{

    public class EnemyAttackBehaviour : NetworkBehaviour
    {
        [SerializeField][Tooltip("硬直時間")] protected float _stiffness;
        [SerializeField][Tooltip("射程")] protected float _range;
        [SerializeField][Tooltip("重み付")] protected float _mass;

        public virtual float GetStiffness()
        {
            return _stiffness;
        }

        public virtual float GetRange()
        {
            return _range;
        }

        public virtual float GetMass()
        {
            return _mass;
        }

        /// <summary>
        /// 攻撃行動を行う
        /// </summary>
        public virtual void Behaviour(PlayerStatus target)
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}