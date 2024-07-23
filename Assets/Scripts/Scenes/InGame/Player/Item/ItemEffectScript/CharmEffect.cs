using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class CharmEffect : ItemEffect
    {
        private bool _isHaveCharm = false;
        public override void OnPickUp()
        {
            ownerPlayerStatus.HaveCharm(true);
        }

        public override void OnThrow()
        {
            //　アイテムスロット内にまだお守りが残っているかどうか判定
            for (int i = 0 ; i < 7 ; i++) {
                var item = ownerPlayerItem.ItemSlots[i];
                if ( i != ownerPlayerItem.nowIndex) {
                    if (item.myItemData != null) {
                        if (item.myItemData.itemID == 6) {
                            _isHaveCharm = true;
                        }
                    }
                }
            }
            //お守りが残っていなければ効果消滅
            if (!_isHaveCharm) {
                ownerPlayerStatus.HaveCharm(false);
            }          
        }

        public override void Effect()
        {

        }

      
    }
}

