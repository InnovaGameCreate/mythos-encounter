using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class AdrenalineSyringeEffect : ItemEffect
    {
        private float _startTime;
        [SerializeField] private float _animationTime;
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
            StartCoroutine(UseSyringe());
        }


        public IEnumerator UseSyringe()
        {
            Debug.Log("アドレナリン注射器バフ状態");

            //アイテム使用直後にステータス変更を行う
            ownerPlayerStatus.UseItem(true);
            ownerPlayerStatus.ChangeSpeed();
            ownerPlayerItem.ChangeCanChangeBringItem(false);
            //ToDo:Playerに包帯を巻くエフェクトを発生させる

            _startTime = Time.time;
            Debug.Log(_startTime);

            while (true)
            {
                yield return null;
                //攻撃を食らった際にこのコルーチンを破棄              
                if (_stopCoroutineBool == true) {
                    Debug.Log("包帯使用のコルーチンを破棄");
                    _stopCoroutineBool = false;
                    ownerPlayerStatus.UseItem(false);
                    ownerPlayerStatus.ChangeSpeed();
                    ownerPlayerItem.ChangeCanChangeBringItem(true);
                    yield break;
                }

                //注射器を打つアニメーションが終わったら効果発動
                if (Time.time - _startTime >= _animationTime) {               
                    ownerPlayerStatus.UseItem(false);
                    ownerPlayerStatus.ChangeSpeed();
                    ownerPlayerItem.ConsumeItem(ownerPlayerItem.nowIndex);
                    ownerPlayerItem.ChangeCanChangeBringItem(true);
                    ownerPlayerStatus.StartBuff();//アドレナリン状態を変化させるためのコマンド
                    yield break;
                }

            }


        }


    }
}

