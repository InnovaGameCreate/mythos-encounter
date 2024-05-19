public struct EnemyDataStruct
{
    private int _enemyId;
    private string _name;
    private int _hp;
    private int _stamina;
    private int _adArmor;
    private int _mrArmor;
    private int _walkSpeed;
    private int _dashSpeed;
    private int _attackPower;
    private int _actionCooltime;
    private int _hearing;
    private int _vision;
    private string[] _spell;
    private int _spellCount;
    private float _san;

    public int enemyId { get => _enemyId; }//キャラクターID
    public string name { get => _name; }//神話生物名
    public int hp { get => _hp; }//体力
    public int stamina { get => _stamina; }//スタミナ
    public int adArmor { get => _adArmor; }//物理防御
    public int mrArmor { get => _mrArmor; }//魔法防御
    public int walkSpeed { get => _walkSpeed; }//歩行速度
    public int dashSpeed { get => _dashSpeed; }//疾走速度
    public int attackPower { get => _attackPower; }//攻撃力
    public int actionCooltime { get => _actionCooltime; }//アクションクールタイム
    public int hearing { get => _hearing; }//聴覚
    public int bision { get => _vision; }//視覚
    public string[] spell { get => _spell; }//呪文
    public int spellCount { get => _spellCount; }//呪文の覚えている数
    public float san { get => _san; }//視認時減少SAN値

    public void EnemyDataSet(int enemyId, string name, int hp, int stamina, int adArmor, int mrArmor, int walkSpeed, int dashSpeed, int attackPower, int hearing, int vision, int actionCooltime, string[] spell, int spellCount, float san)
    {
        _enemyId = enemyId;
        _name = name;
        _hp = hp;
        _stamina = stamina;
        _adArmor = adArmor;
        _mrArmor = mrArmor;
        _walkSpeed = walkSpeed;
        _dashSpeed = dashSpeed;
        _attackPower = attackPower;
        _hearing = hearing;
        _vision = vision;
        _actionCooltime = actionCooltime;
        _spell = spell;
        _spellCount = spellCount;
        _san = san;
    }
}
