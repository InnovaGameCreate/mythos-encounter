using UnityEngine;

/// <summary>
/// アイテムの効果・データを管理する為の抽象クラス
/// 継承するクラスの名前は「アイテム名 + Effect」とすること
/// Startメソッドで必ずSetUp関数を呼ぶこと. 呼び方:「base.SetUp();」
/// </summary>
public abstract class ItemEffect : MonoBehaviour
{
    public ItemData myItemData;


    public void SetUp()
    {
        myItemData.thisItemEffect = this;
        this.gameObject.layer = LayerMask.NameToLayer("Item");
    }

    public ItemData GetItemData()
    {
        return myItemData; 
    }
    /// <summary>
    /// アイテムの効果を実装する関数
    /// </summary>
    public abstract void Effect();
}
