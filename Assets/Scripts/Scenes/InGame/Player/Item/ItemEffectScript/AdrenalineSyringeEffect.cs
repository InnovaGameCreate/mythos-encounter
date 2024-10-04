using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Scenes.Ingame.Player
{
    public class AdrenalineSyringeEffect : ItemEffect
    {
        private float _startTime;
        [SerializeField] private float _animationTime;
        private bool _stopCoroutineBool = false;
        CancellationTokenSource source = new CancellationTokenSource();

        public override void OnPickUp()
        {
            //攻撃くらったときを示すBoolがTrueになったときにアイテム使用を中断
            ownerPlayerStatus.OnEnemyAttackedMe
                .Subscribe(_ =>
                { 
                    source?.Cancel();
                    source?.Dispose();
                }).AddTo(this);
        }

        public override void OnThrow()
        {

        }

        public override void Effect()
        {
            SyringeEffect().Forget();
        }
        
        public async UniTaskVoid SyringeEffect()
        {
            var token = ownerPlayerStatus.GetCancellationTokenOnDestroy();
            Debug.Log("アドレナリン注射器バフ状態");

            //アイテム使用直後にステータス変更を行う
            ownerPlayerStatus.UseItem(true);
            ownerPlayerStatus.ChangeSpeed();
            ownerPlayerItem.ChangeCanChangeBringItem(false);
            //ToDo:Playerに包帯を巻くエフェクトを発生させる

            ownerPlayerStatus.SetStaminaBuff(true);//アドレナリン状態を変化させるためのコマンド

            await UniTask.WaitForSeconds(15f, cancellationToken: token);
            ownerPlayerStatus.ChangeSpeed();
            ownerPlayerStatus.UseItem(false);
            ownerPlayerItem.ChangeCanChangeBringItem(true);
            if (!token.IsCancellationRequested)
            {
                ownerPlayerStatus.SetStaminaBuff(false);//アドレナリン状態を変化させるためのコマンド
                ownerPlayerItem.ConsumeItem(ownerPlayerItem.nowIndex);
            }
        }


    }
}

