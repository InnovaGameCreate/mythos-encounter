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

        public override void Behaviour(PlayerStatus targetStatus)
        {
            Debug.Log("ãﬂê⁄çUåÇÅI");
            SoundManager.Instance.PlaySe("se_punching00", transform.position);
            targetStatus.ChangeHealth(_damage, ChangeValueMode.Damage);
            targetStatus.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
        }
    }

}