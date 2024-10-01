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
        [SerializeField] private int _breedDamage;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Behaviour(PlayerStatus targetStatus)
        {
            Debug.Log("ãﬂê⁄çUåÇÅI");
            targetStatus.ChangeHealth(_damage, ChangeValueMode.Damage);
            targetStatus.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
        }
    }

}