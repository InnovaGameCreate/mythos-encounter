/// <summary>
/// �A�C�e���X���b�g���g�p�ł��邩�ۂ�
/// </summary>
public enum ItemSlotStatus
{ 
    available,
    unavailable
}

public struct ItemSlotStruct
{
    public ItemData myItemData;
    public ItemSlotStatus myItemSlotStatus;

    /// <summary>
    /// �\���̂ɂ���ϐ���ύX����֐�
    /// </summary>
    /// <param name="data"></param>
    /// <param name="status"></param>
    public void ChangeInfo(ItemData data, ItemSlotStatus status)
    { 
        this.myItemData = data;
        this.myItemSlotStatus = status;
    }
}
