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
                    PlayerItem.nowBringItem = Instantiate(_trapFood, PlayerItem.myRightHand.transform.position, _trapFood.transform.rotation);
                    PlayerItem.nowBringItem.transform.parent = PlayerItem.myRightHand.transform;


                    //�A�C�e���ɃA�^�b�`����Ă���Effect�n�̃X�N���v�g�Ɏ擾�҂̏��𗬂��B
                    var effect = PlayerItem.nowBringItem.gameObject.GetComponent<ItemEffect>();
                    effect.ownerPlayerStatus = status;
                    effect.ownerPlayerItem = PlayerItem;
                    effect.OnPickUp();
                    var rigid = PlayerItem.nowBringItem.GetComponent<Rigidbody>();
                    rigid.useGravity = false;//�A�C�e�������������ɏd�͂̉e�����󂯂Ȃ��悤�ɂ���
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
