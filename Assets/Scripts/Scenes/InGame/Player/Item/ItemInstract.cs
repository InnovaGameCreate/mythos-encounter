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
                PlayerItem.nowBringItem = this.gameObject;


                //�擾�����A�C�e������̋߂��Ɉړ�
                this.gameObject.transform.position = PlayerItem.myRightHand.transform.position;
                this.gameObject.transform.parent = PlayerItem.myRightHand.transform;

                //�A�C�e���ɃA�^�b�`����Ă���Effect�n�̃X�N���v�g�Ɏ擾�҂̏��𗬂��B
                var effect = this.gameObject.GetComponent<ItemEffect>();
                effect.ownerPlayerStatus = status;
                effect.ownerPlayerItem = PlayerItem;
                effect.OnPickUp();//�e�A�C�e���̏E�������̏��������s������
                PlayerItem.nowBringItem.GetComponent<Rigidbody>().useGravity = false;//�A�C�e�������������ɏd�͂̉e�����󂯂Ȃ��悤�ɂ���
            }
        }

        /// <summary>
        /// �A�C�e����؂�ւ����ۂɏ��𗬂����߂̊֐�
        /// </summary>
        /// <param name="status">�������PlayerStatus</param>
        public void InstantIntract(PlayerStatus status)
        {
            var PlayerItem = status.gameObject.GetComponent<PlayerItem>();
            //�A�C�e���ɃA�^�b�`����Ă���Effect�n�̃X�N���v�g�Ɏ擾�҂̏��𗬂��B
            var effect = this.gameObject.GetComponent<ItemEffect>();
            effect.ownerPlayerStatus = status;
            effect.ownerPlayerItem = PlayerItem;
            effect.OnPickUp();//�e�A�C�e���̏E�������̏��������s������
            PlayerItem.nowBringItem.GetComponent<Rigidbody>().useGravity = false;//�A�C�e�������������ɏd�͂̉e�����󂯂Ȃ��悤�ɂ���
        }
    }
}
