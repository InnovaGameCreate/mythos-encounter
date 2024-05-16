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
                PlayerItem.nowBringItem = this.gameObject;


                //取得したアイテムを手の近くに移動
                this.gameObject.transform.position = PlayerItem.myRightHand.transform.position;
                this.gameObject.transform.parent = PlayerItem.myRightHand.transform;

                //アイテムにアタッチされているEffect系のスクリプトに取得者の情報を流す。
                var effect = this.gameObject.GetComponent<ItemEffect>();
                effect.ownerPlayerStatus = status;
                effect.ownerPlayerItem = PlayerItem;
                effect.OnPickUp();//各アイテムの拾った時の処理を実行させる
                PlayerItem.nowBringItem.GetComponent<Rigidbody>().useGravity = false;//アイテムを持った時に重力の影響を受けないようにする
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
    }
}
