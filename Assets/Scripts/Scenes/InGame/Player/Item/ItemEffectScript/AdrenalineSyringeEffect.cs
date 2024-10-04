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
            //�U����������Ƃ�������Bool��True�ɂȂ����Ƃ��ɃA�C�e���g�p�𒆒f
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
            Debug.Log("�A�h���i�������ˊ�o�t���");

            //�A�C�e���g�p����ɃX�e�[�^�X�ύX���s��
            ownerPlayerStatus.UseItem(true);
            ownerPlayerStatus.ChangeSpeed();
            ownerPlayerItem.ChangeCanChangeBringItem(false);
            //ToDo:Player�ɕ�т������G�t�F�N�g�𔭐�������

            ownerPlayerStatus.SetStaminaBuff(true);//�A�h���i������Ԃ�ω������邽�߂̃R�}���h

            await UniTask.WaitForSeconds(15f, cancellationToken: token);
            ownerPlayerStatus.ChangeSpeed();
            ownerPlayerStatus.UseItem(false);
            ownerPlayerItem.ChangeCanChangeBringItem(true);
            if (!token.IsCancellationRequested)
            {
                ownerPlayerStatus.SetStaminaBuff(false);//�A�h���i������Ԃ�ω������邽�߂̃R�}���h
                ownerPlayerItem.ConsumeItem(ownerPlayerItem.nowIndex);
            }
        }


    }
}

