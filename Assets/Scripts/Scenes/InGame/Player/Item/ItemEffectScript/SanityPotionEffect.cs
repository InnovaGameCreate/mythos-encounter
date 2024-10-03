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
            //�U����������Ƃ�������Bool��True�ɂȂ����Ƃ��ɃA�C�e���g�p�𒆒f
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
                Debug.Log("SAN�l���ő�Ȃ̂Ŏg�p�s��");
            }

        }

        public IEnumerator UseSanityPotion()
        {
            Debug.Log("SAN�l�񕜖�g��");

            //�A�C�e���g�p����ɃX�e�[�^�X�ύX���s��
            ownerPlayerStatus.UseItem(true);
            ownerPlayerStatus.ChangeSpeed();
            ownerPlayerItem.ChangeCanChangeBringItem(false);

            _startTime = Time.time;
            _lapTime = Time.time;

            Debug.Log(_startTime);

            while (true)
            {
                yield return null;
                //�U����H������ۂɂ��̃R���[�`����j��              
                if (_stopCoroutineBool == true)
                {
                    Debug.Log("san�񕜖�g�p�̃R���[�`����j��");
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

