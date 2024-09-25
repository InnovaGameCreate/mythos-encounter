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
            //��ʏ�ɉ��x�v��\������
            ownerPlayerItem.ActiveGeigerCounter(true);

            //�I���A�C�e����ʂ̂��̂ɂ����Ƃ��A�����ŉ��x�v���\���ɂ���
            ownerPlayerItem.OnNowIndexChange
            .Skip(1)
            .Where(x => ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData == null || ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData.itemID != 20)
            .Subscribe(_ =>
            {
                ownerPlayerItem.ActiveGeigerCounter(false);
            }).AddTo(this);
        }

        public override void OnThrow()
        {
            //�p�����ɕ��ː��������\���ɂ���
            ownerPlayerItem.ActiveGeigerCounter(false);
        }

        public override void Effect()
        {
            //���ː�������on/off��ς���
            ownerPlayerItem.ChangeSwitchGeigerCounter();
        }

    }
}


