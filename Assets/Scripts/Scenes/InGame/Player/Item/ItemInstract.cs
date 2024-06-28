using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace Scenes.Ingame.Player
{
    public class ItemInstract : MonoBehaviour, IInteractable
    {
        public void Intract(PlayerStatus status)
        {
            var PlayerItem = status.gameObject.GetComponent<PlayerItem>();
            bool isPickedUp = false;

            if (Input.GetMouseButtonDown(1))
            {
                ItemSlotStruct item = new ItemSlotStruct();
                item.ChangeInfo(this.GetComponent<ItemEffect>().GetItemData(), ItemSlotStatus.available);

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
                    PlayerItem.nowBringItem = this.gameObject;


                    //取得したアイテムを手の近くに移動
                    this.gameObject.transform.position = PlayerItem.myRightHand.transform.position;
                    this.gameObject.transform.parent = PlayerItem.myRightHand.transform;

                    //アイテムにアタッチされているEffect系のスクリプトに取得者の情報を流す。
                    var effect = this.gameObject.GetComponent<ItemEffect>();
                    effect.ownerPlayerStatus = status;
                    effect.ownerPlayerItem = PlayerItem;
                    effect.OnPickUp();//各アイテムの拾った時の処理を実行させる
                    var rigid = PlayerItem.nowBringItem.GetComponent<Rigidbody>();
                    rigid.useGravity = false;//アイテムを持った時に重力の影響を受けないようにする
                    rigid.isKinematic = true;
                }
                else
                { 
                    Destroy(this.gameObject);
                }
            }
        }

        /// <summary>
        /// アイテムを切り替えた際に情報を流すための関数
        /// </summary>
        /// <param name="status">持ち主のPlayerStatus</param>
        public void InstantIntract(PlayerStatus status)
        {
            var PlayerItem = status.gameObject.GetComponent<PlayerItem>();
            //アイテムにアタッチされているEffect系のスクリプトに取得者の情報を流す。
            var effect = this.gameObject.GetComponent<ItemEffect>();
            effect.ownerPlayerStatus = status;
            effect.ownerPlayerItem = PlayerItem;
            effect.OnPickUp();//各アイテムの拾った時の処理を実行させる
            PlayerItem.nowBringItem.GetComponent<Rigidbody>().useGravity = false;//アイテムを持った時に重力の影響を受けないようにする
        }

        public string ReturnPopString()
        {
            //このスクリプトでは使わない
            return null;
        }
    }
}
