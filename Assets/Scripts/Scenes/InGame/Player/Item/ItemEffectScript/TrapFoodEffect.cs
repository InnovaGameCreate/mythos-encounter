using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UIElements;
using System;
using static UnityEngine.Rendering.DebugUI;

namespace Scenes.Ingame.Player
{
    public class TrapFoodEffect : ItemEffect
    {

        public override void OnPickUp()
        {
            ownerPlayerItem.StartCoroutine("CreateTrapFood");

            //�I���A�C�e����ʂ̂��̂ɂ����Ƃ��A�����Ńv���r���[���폜����
            ownerPlayerItem.OnNowIndexChange
                .Skip(1)
                .Where(_ => ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData == null || ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData.itemID != 20)
                .Subscribe(_ =>
                {
                    ownerPlayerItem. StopCoroutine("CreateTrapFood");
                    if (ownerPlayerItem.CreatedTrapFood != null)
                    {
                        ownerPlayerItem.ChangeActiveTrapFood(false);
                    }
                }).AddTo(this);
    }

    public override void OnThrow()
        {
            ownerPlayerItem.StopCoroutine("CreateTrapFood");
            if (ownerPlayerItem.CreatedTrapFood != null)
            {
                ownerPlayerItem.DestroyTrapFood();
            }
        }

        public override void Effect()
        {
            if (ownerPlayerItem.IsCanCreateTrapFood && ownerPlayerItem.CreatedTrapFood != null)
            {
                Debug.Log("�A�C�e���ݒu");
                ownerPlayerItem.StopCoroutine("CreateTrapFood");
                ownerPlayerItem.PutTrapFood();
            }
            else
            {
                Debug.Log("�A�C�e���ݒu�s��");
            }
        
        }
    }
}
