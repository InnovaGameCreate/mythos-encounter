using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Player;
using Fusion;

namespace Scenes.Ingame.Enemy
{
    public class EnemyShootingAtackBehaviour : EnemyAttackBehaviour
    {
        [SerializeField] private GameObject _bullet;
        [SerializeField][Tooltip("’e‚Ìo‚Ä‚­‚éêŠ")] private GameObject _hand;
        
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
            Debug.Log("‰“ŠuUŒ‚I");
            Runner.Spawn(_bullet, _hand.transform.position, _bullet.transform.rotation).GetComponent<EnemyBallet>().Init(targetStatus.gameObject.GetComponent<NetworkObject>().Id);
            /*
            target.ChangeHealth(_damage, "Damage");
            target.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
            */
        }
    }
}
