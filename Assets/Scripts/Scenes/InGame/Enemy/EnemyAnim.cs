using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵キャラクターのアニメーション管理基底クラス
    /// </summary>
   
    public abstract class EnemyAnim : MonoBehaviour
    {
        protected Animator _animator;
        protected EnemyStatus _enemyStatus;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _enemyStatus = transform.parent.GetComponent<EnemyStatus>();
        }

        protected virtual void Update()
        {
            UpdateAnimationState();
        }

        protected abstract void UpdateAnimationState();

    }
}