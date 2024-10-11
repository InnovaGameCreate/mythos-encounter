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
        [SerializeField][Tooltip("�d������")] protected float _stiffness;
        [SerializeField][Tooltip("�˒�")] protected float _range;
        [SerializeField][Tooltip("�d�ݕt")] protected float _mass;

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
        /// �U���s�����s��
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