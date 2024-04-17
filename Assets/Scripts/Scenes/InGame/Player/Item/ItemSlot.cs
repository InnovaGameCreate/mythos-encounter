/// <summary>
/// アイテムスロットが使用できるか否か
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
    /// 構造体にある変数を変更する関数
    /// </summary>
    /// <param name="data"></param>
    /// <param name="status"></param>
    public void ChangeInfo(ItemData data, ItemSlotStatus status)
    { 
        this.myItemData = data;
        this.myItemSlotStatus = status;
    }
}
