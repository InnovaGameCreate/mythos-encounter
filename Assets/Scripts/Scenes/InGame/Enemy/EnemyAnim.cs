using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵キャラクターのアニメーション管理基底クラス
    /// </summary>
    public abstract class EnemyAnim : MonoBehaviour
    {
        protected Animator _animator;
        protected EnemyStatus _enemyStatus;
        protected EnemyMove _enemyMove;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _enemyStatus = GetComponent<EnemyStatus>();
            _enemyMove = GetComponent<EnemyMove>();
        }

        protected virtual void FixedUpdate()
        {
            UpdateAnimationState();
        }

        protected abstract void UpdateAnimationState();
    }
}