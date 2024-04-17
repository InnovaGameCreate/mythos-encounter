using Scenes.Ingame.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("�A�C�e���̌ŗL�l")]
    public int itemID;
    public string itemName;
    public Sprite itemSprite;

    [Header("�V���b�v�֘A�̏��")]
    public int price;
    public string itemInfo;
    public ItemType type;

    [Header("�C���Q�[���̐ݒ�")]
    [Tooltip("�g���؂�̃A�C�e���Ȃ�true��")]
    public bool isSingleUse;
    public ItemEffect thisItemEffect;//inspecter��ł͋�̂܂܂ő��v
}

