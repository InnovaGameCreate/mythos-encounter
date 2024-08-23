using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Player;

namespace Scenes.Ingame.Enemy
{
    public class EnemyShootingAtackBehaviour : EnemyAttackBehaviour
    {
        [SerializeField] private int _damage;
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
            GameObject.Instantiate(_bullet,_hand.transform.position,_bullet.transform.rotation).GetComponent<EnemyBallet>().Init(targetStatus);
            /*
            target.ChangeHealth(_damage, "Damage");
            target.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
            */
        }
    }
}
