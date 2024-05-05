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
    public string name { get => _name; }//名前
    public int money { get => _money; }//所持金
    public string[] items { get => _items; }//アイテム
    public int mythPoint { get => _mythPoint; }//神話ポイント
    public string[] spell { get => _spell; }//取得した呪文
    public int spellCount { get => _spell.Length; }//取得した呪文の種類数
    public string[] mythCreature { get => _mythCreature; }//遭遇した神話生物
    public int mythCreatureCount { get => _mythCreature.Length; }//遭遇した神話生物の種類の数
    public int escape { get => _escape; }//敵を討伐せずにクリアした回数
    public int dispersingEscape { get => _dispersingEscape; }//敵を討伐してクリアした数
    public int stageClearCount { get => _dispersingEscape + _escape; }//ステージのクリア回数

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
