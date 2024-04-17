using Scenes.Ingame.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("アイテムの固有値")]
    public int itemID;
    public string itemName;
    public Sprite itemSprite;

    [Header("ショップ関連の情報")]
    public int price;
    public string itemInfo;
    public ItemType type;

    [Header("インゲームの設定")]
    [Tooltip("使い切りのアイテムならtrueに")]
    public bool isSingleUse;
    public ItemEffect thisItemEffect;//inspecter上では空のままで大丈夫
}

