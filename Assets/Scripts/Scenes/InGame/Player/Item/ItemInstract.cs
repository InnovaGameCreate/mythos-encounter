using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    public class ItemInstract : MonoBehaviour,IInteractable
    {
        public void Intract(PlayerStatus status)
        {
            var PlayerItem = status.gameObject.GetComponent<PlayerItem>();


            if (PlayerItem.ItemSlots[PlayerItem.nowIndex].myItemData == null && PlayerItem.ItemSlots[PlayerItem.nowIndex].myItemSlotStatus == ItemSlotStatus.available && Input.GetMouseButtonDown(1))
            {
                ItemSlotStruct item = new ItemSlotStruct();
                item.ChangeInfo(this.GetComponent<ItemEffect>().GetItemData(), ItemSlotStatus.available);
                PlayerItem.ChangeListValue(PlayerItem.nowIndex, item);//アイテムスロットにアイテムを格納

                StartCoroutine(PlayerItem.CanUseItem());//アイテム取得後0.1秒はアイテムを使えないようにする。
                Destroy(this.gameObject , 0.15f);//ステージ上にあるアイテムを破壊
            }
        }
    }
}
