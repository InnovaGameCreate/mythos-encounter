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
            //画面上に温度計を表示する
            ownerPlayerItem.ActiveGeigerCounter(true);

            //選択アイテムを別のものにしたとき、自動で温度計を非表示にする
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
            //廃棄時に放射線測定器を非表示にする
            ownerPlayerItem.ActiveGeigerCounter(false);
        }

        public override void Effect()
        {
            //放射線測定器のon/offを変える
            ownerPlayerItem.ChangeSwitchGeigerCounter();
        }

    }
}


