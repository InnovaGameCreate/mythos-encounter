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
            //�U����������Ƃ�������Bool��True�ɂȂ����Ƃ��ɃA�C�e���g�p�𒆒f
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
            Debug.Log("�A�h���i�������ˊ�o�t���");

            //�A�C�e���g�p����ɃX�e�[�^�X�ύX���s��
            ownerPlayerStatus.UseItem(true);
            ownerPlayerStatus.ChangeSpeed();
            ownerPlayerItem.ChangeCanChangeBringItem(false);
            //ToDo:Player�ɕ�т������G�t�F�N�g�𔭐�������

            _startTime = Time.time;
            Debug.Log(_startTime);

            while (true)
            {
                yield return null;
                //�U����H������ۂɂ��̃R���[�`����j��              
                if (_stopCoroutineBool == true) {
                    Debug.Log("��юg�p�̃R���[�`����j��");
                    _stopCoroutineBool = false;
                    ownerPlayerStatus.UseItem(false);
                    ownerPlayerStatus.ChangeSpeed();
                    ownerPlayerItem.ChangeCanChangeBringItem(true);
                    yield break;
                }

                //���ˊ��łA�j���[�V�������I���������ʔ���
                if (Time.time - _startTime >= _animationTime) {               
                    ownerPlayerStatus.UseItem(false);
                    ownerPlayerStatus.ChangeSpeed();
                    ownerPlayerItem.ConsumeItem(ownerPlayerItem.nowIndex);
                    ownerPlayerItem.ChangeCanChangeBringItem(true);
                    ownerPlayerStatus.StartBuff();//�A�h���i������Ԃ�ω������邽�߂̃R�}���h
                    yield break;
                }

            }


        }


    }
}

