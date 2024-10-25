using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class HandLIghtEffect : ItemEffect
    {
       
        public override void OnPickUp()
        {
            if (ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.NotActive) //�A�C�e���擾��
            {
                ownerPlayerItem.ActiveHandLight(true);
                ownerPlayerItem.ChangeSwitchHandLight(HandLightState.On);
            }
            else//�A�C�e���I����
            {
                //�ȑO�̏�Ԃ��烉�C�g�̋N���E��~�����肷��
                if (ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.Off)
                {
                    ownerPlayerItem.ActiveHandLight(false);
                }
                else if (ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.On)
                {
                    ownerPlayerItem.ActiveHandLight(true);
                }
            }

            //�I���A�C�e����ʂ̂��̂ɂ����Ƃ��A�����Ń��C�g���~����
            ownerPlayerItem.OnNowIndexChange
                .Skip(1)
                .Where(x => ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData == null || ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData.itemID != 3)
                .Subscribe(_ =>
                {
                    ownerPlayerItem.ActiveHandLight(false);
                }).AddTo(this);

        }

        public override void OnThrow()
        {
            //�A�C�e���p�����Ƀ��C�g���~����
            ownerPlayerItem.ActiveHandLight(false);
            ownerPlayerItem.ChangeSwitchHandLight(HandLightState.NotActive);
        }

        public override void Effect()
        {
            //���N���b�N���Ƀ��C�g��ON/OFF��Ԃ�؂�ւ��A�N���E��~����
            SoundManager.Instance.PlaySe("se_switch00", transform.position);
            if (ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.Off)
            {
                ownerPlayerItem.ChangeSwitchHandLight(HandLightState.On);
                ownerPlayerItem.ActiveHandLight(true);
            }
            else if(ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.On)
            {
                ownerPlayerItem.ChangeSwitchHandLight(HandLightState.Off);
                ownerPlayerItem.ActiveHandLight(false);
            }

        }

     
        
    }
}

