using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class EnemyUniqueAction : MonoBehaviour
    {
        private float _actionCoolDownTime;
        private float _actionCoolDownTimeCount = 0;

        protected virtual void Start() { }

        public virtual void Init(int actionCoolDown) {
            _actionCoolDownTime = (float)actionCoolDown;
        }


        // Update is called once per frame
        protected virtual void Update()
        {
            _actionCoolDownTimeCount += Time.deltaTime;
            if (_actionCoolDownTimeCount > _actionCoolDownTime)
            {
                _actionCoolDownTimeCount -= _actionCoolDownTime;
                Action();
            }
        }
        protected virtual void Action() {
            Debug.LogWarning("設定されていないアクションが実行されました");
        }
    }
}
