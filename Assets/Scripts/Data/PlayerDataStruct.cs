public struct PlayerDataStruct
{
    private int _recordId;
    private string _name;
    private int _money;
    private string[] _items;
    private int _mythPoint;
    private string[] _spell;
    private string[] _mythCreature;
    private int _escape;
    private int _dispersingEscape;


    public int recordId { get => _recordId; }//ID
    public string name { get => _name; }//���O
    public int money { get => _money; }//������
    public string[] items { get => _items; }//�A�C�e��
    public int mythPoint { get => _mythPoint; }//�_�b�|�C���g
    public string[] spell { get => _spell; }//�擾��������
    public int spellCount { get => _spell.Length; }//�擾���������̎�ސ�
    public string[] mythCreature { get => _mythCreature; }//���������_�b����
    public int mythCreatureCount { get => _mythCreature.Length; }//���������_�b�����̎�ނ̐�
    public int escape { get => _escape; }//�G�𓢔������ɃN���A������
    public int dispersingEscape { get => _dispersingEscape; }//�G�𓢔����ăN���A������
    public int stageClearCount { get => _dispersingEscape + _escape; }//�X�e�[�W�̃N���A��

    public void PlayerDataSet(int recordId, string name, int money, string[] items, int mythPoint, string[] spell, string[] mythCreature, int escape, int dispersingEscape)
    {
        _recordId = recordId;
        _name = name;
        _money = money;
        _items = items;
        _mythPoint = mythPoint;
        _spell = spell;
        _mythCreature = mythCreature;
        _escape = escape;
        _dispersingEscape = dispersingEscape;
    }
}
