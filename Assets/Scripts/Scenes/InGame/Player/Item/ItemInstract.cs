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
                PlayerItem.ChangeListValue(PlayerItem.nowIndex, item);//�A�C�e���X���b�g�ɃA�C�e�����i�[

                StartCoroutine(PlayerItem.CanUseItem());//�A�C�e���擾��0.1�b�̓A�C�e�����g���Ȃ��悤�ɂ���B
                Destroy(this.gameObject , 0.15f);//�X�e�[�W��ɂ���A�C�e����j��
            }
        }
    }
}
