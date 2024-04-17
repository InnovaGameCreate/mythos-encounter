using UnityEngine;

/// <summary>
/// �A�C�e���̌��ʁE�f�[�^���Ǘ�����ׂ̒��ۃN���X
/// �p������N���X�̖��O�́u�A�C�e���� + Effect�v�Ƃ��邱��
/// Start���\�b�h�ŕK��SetUp�֐����ĂԂ���. �Ăѕ�:�ubase.SetUp();�v
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
    /// �A�C�e���̌��ʂ���������֐�
    /// </summary>
    public abstract void Effect();
}
