using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Scenes.Ingame.Player
{
    public class GeigerCounterEffect : ItemEffect
    {
        public override void OnPickUp()
        {
            //��ʏ�ɕ��ː�������\������
            SoundManager.Instance.PlaySe("se_geigercounter00", transform.position);
            ownerPlayerItem.ActiveGeigerCounter(true);
            //ownerPlayerItem.UseGeigerCounter(false);
            if (ownerPlayerItem.SwitchGeigerCounter[ownerPlayerItem.nowIndex] == true)// �d����on�ł��鎞
            {
                ownerPlayerItem.UseGeigerCounter(true);
            }

            //�I���A�C�e����ʂ̂��̂ɂ����Ƃ��A�����ő���@���\���ɂ���
            ownerPlayerItem.OnNowIndexChange
                .Skip(1)
                .Where(x => ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData == null || ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData.itemID != 19)
                .Subscribe(_ =>
                {
                    ownerPlayerItem.ActiveGeigerCounter(false);
                }).AddTo(this);
        }

        public override void OnThrow()
        {
            //�p�����ɑ������\���ɂ���
            ownerPlayerItem.ChangeSwitchGeigerCounter(false);
            ownerPlayerItem.ActiveGeigerCounter(false);
        }

        public override void Effect()
        {
            if (ownerPlayerItem.SwitchGeigerCounter[ownerPlayerItem.nowIndex] == true)// ����킪���蒆�̏ꍇ
            {
                ownerPlayerItem.UseGeigerCounter(false);
                ownerPlayerItem.ChangeSwitchGeigerCounter(false);
            }
            else
            {
                ownerPlayerItem.UseGeigerCounter(true);
                ownerPlayerItem.ChangeSwitchGeigerCounter(true);
            }
        }

    }
}


