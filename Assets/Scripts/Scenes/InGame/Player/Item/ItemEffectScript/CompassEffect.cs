using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class CompassEffect : ItemEffect
    {

        public override void OnPickUp()
        {
            //画面上にコンパスを表示する
            ownerPlayerItem.ActiveCompass(true);

            //選択アイテムを別のものにしたとき、自動でコンパスを非表示にする
            ownerPlayerItem.OnNowIndexChange
            .Skip(1)
            .Where(x => ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData == null || ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData.itemID != 4)
            .Subscribe(_ =>
            {
                ownerPlayerItem.ActiveCompass(false);
            });
        }

        public override void OnThrow()
        {
            //廃棄時にコンパスを非表示にする
            ownerPlayerItem.ActiveCompass(false);
        }

        public override void Effect()
        {

        }


    }
}


