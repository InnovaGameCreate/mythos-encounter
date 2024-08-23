using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
namespace Scenes.Ingame.Enemy
{

    public class EnemyCloseAtackBehaviour : EnemyAttackBehaviour
    {
        [SerializeField] private int _damage;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void Behaviour(PlayerStatus target)
        {
            Debug.Log("ãﬂê⁄çUåÇÅI");
            target.ChangeHealth(_damage, "Damage");
            target.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
        }
    }

}