using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scenes.Ingame.Player;
using UniRx;

namespace Scenes.Ingame.Enemy
{
    public class EnemyHitScanBullet : MonoBehaviour
    {
        [SerializeField] private int _damage;
        private GameObject _target;
        private PlayerStatus _targetStatus;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 発射時にアクセスする項目
        /// </summary>
        /// <param name="target"></param>
        public void Init(GameObject target, PlayerStatus targetStatus)
        {
            _target = target;
            _targetStatus = targetStatus;
            this.transform.LookAt(_target.transform.position);
            _targetStatus.ChangeHealth(_damage, "Damage");
            _targetStatus.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
        }
    }
}
