using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class SanityPotionEffect : ItemEffect
    {
        private float _startTime;
        private float _lapTime;
        private bool _stopCoroutineBool = false;


        public override void OnPickUp()
        {
            //攻撃くらったときを示すBoolがTrueになったときにアイテム使用を中断
            ownerPlayerStatus.OnEnemyAttackedMe
                .Subscribe(_ => _stopCoroutineBool = true).AddTo(this);
        }

        public override void OnThrow()
        {

        }

        public override void Effect()
        {
            if(ownerPlayerStatus.nowPlayerSanValue != 100)
            {
                StartCoroutine(UseSanityPotion());
            }
            else
            {
                Debug.Log("SAN値が最大なので使用不可");
            }

        }

        public IEnumerator UseSanityPotion()
        {
            Debug.Log("SAN値回復薬使う");

            //アイテム使用直後にステータス変更を行う
            ownerPlayerStatus.UseItem(true);
            ownerPlayerStatus.ChangeSpeed();
            ownerPlayerItem.ChangeCanChangeBringItem(false);

            _startTime = Time.time;
            _lapTime = Time.time;

            Debug.Log(_startTime);

            while (true)
            {
                yield return null;
                //攻撃を食らった際にこのコルーチンを破棄              
                if (_stopCoroutineBool == true)
                {
                    Debug.Log("san回復薬使用のコルーチンを破棄");
                    _stopCoroutineBool = false;
                    ownerPlayerStatus.UseItem(false);
                    ownerPlayerStatus.ChangeSpeed();
                    ownerPlayerItem.ConsumeItem(ownerPlayerItem.nowIndex);
                    ownerPlayerItem.ChangeCanChangeBringItem(true);
                    yield break;
                }

                if(Time.time - _lapTime >= 0.45f)
                {
                    ownerPlayerStatus.ChangeSanValue(1, ChangeValueMode.Heal);
                    _lapTime = Time.time;
                }

                if (Time.time - _startTime >= 5.0f || ownerPlayerStatus.nowPlayerSanValue == 100)
                {
                    ownerPlayerStatus.UseItem(false);
                    ownerPlayerStatus.ChangeSpeed();
                    ownerPlayerItem.ConsumeItem(ownerPlayerItem.nowIndex);
                    ownerPlayerItem.ChangeCanChangeBringItem(true);
                    yield break;
                }
            }
        }
    }
}

