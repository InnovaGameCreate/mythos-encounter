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
            //��юg�p���Ƀ_���[�W���󂯂����т̎g�p������
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
            Debug.Log("��юg��");
            //�A�C�e���d�l����ɃX�e�[�^�X�ύX���s��
            int speed = ownerPlayerStatus.nowPlayerSpeed;
            ownerPlayerStatus.ChangeSpeed(ownerPlayerStatus.nowPlayerSpeed / 2);
            //ToDo:Player�ɕ�т������G�t�F�N�g�𔭐�������

            
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
   
