using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    public class TrapFoodInstract : ItemInstract
    {
        [SerializeField] private GameObject _trapFood;
        public override void Intract(PlayerStatus status, bool processWithConditionalBypass)
        {
            var PlayerItem = status.gameObject.GetComponent<PlayerItem>();
            bool isPickedUp = false;

            if (Input.GetMouseButtonDown(1))
            {
                ItemSlotStruct item = new ItemSlotStruct();
                item.ChangeInfo(_trapFood.GetComponent<ItemEffect>().GetItemData(), ItemSlotStatus.available);

                for (int i = 0; i < 7; i++)
                {
                    int index = PlayerItem.nowIndex + i;
                    if (index > 6)
                        index -= 7;

                    if (PlayerItem.ItemSlots[index].myItemData == null && PlayerItem.ItemSlots[index].myItemSlotStatus == ItemSlotStatus.available)
                    {
                        PlayerItem.ChangeListValue(index, item);//アイテムスロットにアイテムを格納
                        isPickedUp = true;
                        break;
                    }
                    else
                        continue;
                }

                //このアイテムが拾えなかったら終了
                if (!isPickedUp)
                    return;

                if (PlayerItem.nowBringItem == null)
                {
                    PlayerItem.nowBringItem = Instantiate(_trapFood, PlayerItem.myRightHand.transform.position, _trapFood.transform.rotation);
                    PlayerItem.nowBringItem.transform.parent = PlayerItem.myRightHand.transform;


                    //アイテムにアタッチされているEffect系のスクリプトに取得者の情報を流す。
                    var effect = PlayerItem.nowBringItem.gameObject.GetComponent<ItemEffect>();
                    effect.ownerPlayerStatus = status;
                    effect.ownerPlayerItem = PlayerItem;
                    effect.OnPickUp();
                    var rigid = PlayerItem.nowBringItem.GetComponent<Rigidbody>();
                    rigid.useGravity = false;//アイテムを持った時に重力の影響を受けないようにする
                    rigid.isKinematic = true;
                    Destroy(this.gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
