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
            //画面上に放射線測定器を表示する
            ownerPlayerItem.ActiveGeigerCounter(true);
            //ownerPlayerItem.UseGeigerCounter(false);
            if (ownerPlayerItem.SwitchGeigerCounter[ownerPlayerItem.nowIndex] == true)// 電源がonである時
            {
                ownerPlayerItem.UseGeigerCounter(true);
            }

            //選択アイテムを別のものにしたとき、自動で測定機を非表示にする
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
            //廃棄時に測定器を非表示にする
            ownerPlayerItem.ChangeSwitchGeigerCounter(false);
            ownerPlayerItem.ActiveGeigerCounter(false);
        }

        public override void Effect()
        {
            if (ownerPlayerItem.SwitchGeigerCounter[ownerPlayerItem.nowIndex] == true)// 測定器が測定中の場合
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


