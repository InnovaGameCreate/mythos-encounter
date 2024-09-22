using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Scenes.Ingame.Player
{
    public class ThermoeterEffect : ItemEffect
    {
        public override void OnPickUp()
        {
            //画面上に温度計を表示する
            ownerPlayerItem.ActiveThermometer(true);

            //選択アイテムを別のものにしたとき、自動で温度計を非表示にする
            ownerPlayerItem.OnNowIndexChange
            .Skip(1)
            .Where(x => ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData == null || ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData.itemID != 20)
            .Subscribe(_ =>
            {
                ownerPlayerItem.ActiveThermometer(false);
            }).AddTo(this);
        }

        public override void OnThrow()
        {
            //廃棄時に温度計を非表示にする
            ownerPlayerItem.ActiveThermometer(false);
        }

        public override void Effect()
        {
            ownerPlayerItem.UseThermometer();    
            ownerPlayerItem.ChangeCanUseItem(false);                   
        }

    }
}


