using UnityEngine;
namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �A�C�e���̌��ʁE�f�[�^���Ǘ�����ׂ̒��ۃN���X
    /// �q�N���X�̖��O�́u�A�C�e���� + Effect�v�Ƃ��邱��
    /// Start���\�b�h�ŕK��SetUp�֐����ĂԂ���. �Ăѕ�:�ubase.SetUp();�v
    /// 
    /// </summary>
    [RequireComponent(typeof(ItemInstract))]
    public abstract class ItemEffect : MonoBehaviour
    {
        public ItemData myItemData;
        [HideInInspector]public PlayerStatus ownerPlayerStatus;
        [HideInInspector] public PlayerItem ownerPlayerItem;

        public void SetUp()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Item");
        }

        public ItemData GetItemData()
        {
            return myItemData;
        }

        /// <summary>
        /// �E��ꂽ�Ƃ��Ɏ��s���鏈�����L�q
        /// </summary>
        public abstract void OnPickUp();

        /// <summary>
        /// �A�C�e���̌��ʂ���������֐�
        /// �A�C�e�����g���I�������uownerPlayerItem.ThrowItem(ownerPlayerItem.nowIndex);�v���L�q���邱��
        /// </summary>
        public abstract void Effect();
    }
}

