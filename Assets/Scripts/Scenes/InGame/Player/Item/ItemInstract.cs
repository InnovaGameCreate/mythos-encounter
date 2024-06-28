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
                        PlayerItem.ChangeListValue(index, item);//�A�C�e���X���b�g�ɃA�C�e�����i�[
                        isPickedUp = true;
                        break;
                    }
                    else
                        continue;
                }

                //���̃A�C�e�����E���Ȃ�������I��
                if (!isPickedUp)
                    return;

                if (PlayerItem.nowBringItem == null)
                {
                    PlayerItem.nowBringItem = this.gameObject;


                    //�擾�����A�C�e������̋߂��Ɉړ�
                    this.gameObject.transform.position = PlayerItem.myRightHand.transform.position;
                    this.gameObject.transform.parent = PlayerItem.myRightHand.transform;

                    //�A�C�e���ɃA�^�b�`����Ă���Effect�n�̃X�N���v�g�Ɏ擾�҂̏��𗬂��B
                    var effect = this.gameObject.GetComponent<ItemEffect>();
                    effect.ownerPlayerStatus = status;
                    effect.ownerPlayerItem = PlayerItem;
                    effect.OnPickUp();//�e�A�C�e���̏E�������̏��������s������
                    var rigid = PlayerItem.nowBringItem.GetComponent<Rigidbody>();
                    rigid.useGravity = false;//�A�C�e�������������ɏd�͂̉e�����󂯂Ȃ��悤�ɂ���
                    rigid.isKinematic = true;
                }
                else
                { 
                    Destroy(this.gameObject);
                }
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

        public string ReturnPopString()
        {
            //���̃X�N���v�g�ł͎g��Ȃ�
            return null;
        }
    }
}
