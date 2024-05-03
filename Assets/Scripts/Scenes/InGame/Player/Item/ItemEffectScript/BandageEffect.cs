using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Scenes.Ingame.Player
{
    public class BandageEffect : ItemEffect
    {
        private float _startTime;
        private bool _stopCoroutineBool = false;
        private int speed;
        private void Start()
        {
            base.SetUp();
        }

        public override void OnPickUp()
        {
            //�U����������Ƃ�������Bool��True�ɂȂ����Ƃ��ɃA�C�e���g�p�𒆒f
            ownerPlayerStatus.OnEnemyAttackedMe
                .Skip(1)//�������̎��͖���
                .Subscribe(_ => _stopCoroutineBool = true).AddTo(this);
        }

        public override void Effect()
        {
            if (ownerPlayerStatus.nowBleedingValue)
                StartCoroutine(UseBandage());
            else
                Debug.Log("�o����Ԃł͂Ȃ��̂Ŏg�p���܂���ł����B");
        }

        public IEnumerator UseBandage()
        {
            Debug.Log("��юg��");
            
            //�A�C�e���d�l����ɃX�e�[�^�X�ύX���s��
            speed = ownerPlayerStatus.nowPlayerSpeed;
            ownerPlayerStatus.ChangeSpeed(ownerPlayerStatus.nowPlayerSpeed / 2);
            ownerPlayerItem.isCanChangeBringItem = false;
            //ToDo:Player�ɕ�т������G�t�F�N�g�𔭐�������

            _startTime = Time.time;
            Debug.Log(_startTime);
            
            
            
            while (true)
            {
                yield return null;
                //�U����H������ۂɂ��̃R���[�`����j��              
                if (_stopCoroutineBool == true)
                {
                    Debug.Log("��юg�p�̃R���[�`����j��");
                    _stopCoroutineBool = false;
                    ownerPlayerStatus.ChangeSpeed(speed);
                    ownerPlayerItem.ThrowItem(ownerPlayerItem.nowIndex);
                    ownerPlayerItem.isCanChangeBringItem = true;
                    yield break;
                }
               
                if (Time.time - _startTime >= 10.0f) 
                {
                    ownerPlayerStatus.ChangeBleedingBool(false);
                    ownerPlayerStatus.ChangeSpeed(speed);
                    ownerPlayerItem.ThrowItem(ownerPlayerItem.nowIndex);
                    ownerPlayerItem.isCanChangeBringItem = true;
                    yield break;
                }
               
            }
            
        }
    }
}
   
