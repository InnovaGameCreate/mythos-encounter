using System;
using UnityEngine;

namespace Scenes.Ingame.Enemy.Animation
{
    public class SpawnOfCthulhuAnim : EnemyAnim
    {
        private static readonly int IsPatrolling = Animator.StringToHash("isPatrolling");
        private static readonly int IsSearching = Animator.StringToHash("isSearching");
        private static readonly int IsChase = Animator.StringToHash("isChase");
        private static readonly int IsAttack = Animator.StringToHash("isAttack");

        protected override void UpdateAnimationState()
        {
            // �A�j���[�V��������
            switch (_enemyStatus.ReturnEnemyState)
            {
                case EnemyState.Patrolling:
                    _animator.SetTrigger(IsPatrolling);
                    break;
                case EnemyState.Searching:
                    _animator.SetTrigger(IsSearching);
                    break;
                case EnemyState.Chase:
                    _animator.SetTrigger(IsChase);
                    break;
                case EnemyState.Attack:
                    _animator.SetTrigger(IsAttack);
                    break;
                case EnemyState.None:
                    break;
                case EnemyState.FallBack:
                    break;
                case EnemyState.Special:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}