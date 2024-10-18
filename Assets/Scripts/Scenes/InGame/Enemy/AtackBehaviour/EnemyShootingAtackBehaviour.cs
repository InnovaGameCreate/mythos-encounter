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

        public override void Behaviour(PlayerStatus targetStatus)
        {
            Debug.Log("‰“ŠuUŒ‚I");
            SoundManager.Instance.PlaySe("se_harpoon00", transform.position);
            Runner.Spawn(_bullet, _hand.transform.position, _bullet.transform.rotation).GetComponent<EnemyBallet>().Init(targetStatus.gameObject.GetComponent<NetworkObject>().Id);
            /*
            target.ChangeHealth(_damage, "Damage");
            target.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
            */
        }
    }
}
