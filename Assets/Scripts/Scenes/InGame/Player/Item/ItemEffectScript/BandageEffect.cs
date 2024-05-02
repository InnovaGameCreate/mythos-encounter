using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Scenes.Ingame.Player
{
    public class BandageEffect : ItemEffect
    {
        private float _timer;
        private bool _stopCoroutineBool = false;
        private void Start()
        {
            base.SetUp();
        }

        public override void OnPickUp()
        {
            //包帯使用中にダメージを受けたら包帯の使用を解除
            ownerPlayerStatus.OnPlayerHealthChange
                .Where(x => ownerPlayerStatus.lastHP > x)
                .Subscribe(_ => _stopCoroutineBool = true)
                .AddTo(this);
        }

        public override void Effect()
        {
            StartCoroutine(UseBandage());
            //ownerPlayerStatus.ChangeBleedingBool(false);
        }

        public IEnumerator UseBandage()
        {
            Debug.Log("包帯使う");
            //アイテム仕様直後にステータス変更を行う
            int speed = ownerPlayerStatus.nowPlayerSpeed;
            ownerPlayerStatus.ChangeSpeed(ownerPlayerStatus.nowPlayerSpeed / 2);
            //ToDo:Playerに包帯を巻くエフェクトを発生させる

            
            while(true)
            {
                _timer += Time.deltaTime;

                if (_stopCoroutineBool == true)
                {
                    _stopCoroutineBool = false;
                    ownerPlayerStatus.ChangeSpeed(speed);
                    yield break;
                }

                if (_timer > 10.0f) 
                {
                    ownerPlayerStatus.ChangeBleedingBool(false);
                    ownerPlayerStatus.ChangeSpeed(speed);
                }
            }
        }
    }
}
   
