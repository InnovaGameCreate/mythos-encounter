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

        public override void Behaviour(PlayerStatus targetStatus)
        {
            Debug.Log("�ߐڍU���I");
            targetStatus.ChangeHealth(_damage, "Damage");
            targetStatus.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
        }
    }

}