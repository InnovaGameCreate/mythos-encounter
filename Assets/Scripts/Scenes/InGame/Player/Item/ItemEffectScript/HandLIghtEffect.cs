using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class HandLIghtEffect : ItemEffect
    {



        private void Start()
        {
            //�A�C�e���I�����Ƀ��C�g�̋N���E��~��ۑ�����Ă���ON/OFF��Ԃ��猈�肷��
            ownerPlayerItem.ActiveHandLight(ownerPlayerItem.SwitchHandLigtht);
        }

        public override void OnPickUp()
        {
            //�A�C�e���擾���Ƀ��C�g���N������
            ownerPlayerItem.ActiveHandLight(true);
            if (!ownerPlayerItem.SwitchHandLigtht)
            {
                ownerPlayerItem.ChangeSwitchHandLight();
            }

        }

        public override void OnThrow()
        {
            //�A�C�e���p�����Ƀ��C�g���~����
            ownerPlayerItem.ActiveHandLight(false);
        }

        public override void Effect()
        {
            //���N���b�N���Ƀ��C�g��ON/OFF��Ԃ�؂�ւ��A�N���E��~����
            ownerPlayerItem.ChangeSwitchHandLight();
            ownerPlayerItem.ActiveHandLight(ownerPlayerItem.SwitchHandLigtht);
        }

     
        
    }
}

