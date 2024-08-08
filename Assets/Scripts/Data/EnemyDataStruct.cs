public struct EnemyDataStruct
{
    private int _enemyId;
    private string _name;
    private int _hp;
    private int _stamina;
    private int _armor;
    private float _walkSpeed;
    private float _dashSpeed;
    private int _attackPower;
    private int _actionCooltime;
    private int _hearing;
    private int _vision;
    private string[] _spell;
    private float _san;

    public int EnemyId { get => _enemyId; }//キャラクターID
    public string Name { get => _name; }//神話生物名
    public int Hp { get => _hp; }//体力
    public int Stamina { get => _stamina; }//スタミナ
    public int Armor { get => _armor; }//防御
    public float WalkSpeed { get => _walkSpeed; }//歩行速度
    public float DashSpeed { get => _dashSpeed; }//疾走速度
    public int AttackPower { get => _attackPower; }//攻撃力
    public int ActionCooltime { get => _actionCooltime; }//アクションクールタイム
    public int Hearing { get => _hearing; }//聴覚
    public int Vision { get => _vision; }//視覚
    public string[] Spell { get => _spell; }//呪文
    public float San { get => _san; }//視認時減少SAN値

    public void EnemyDataSet(int enemyId, string name, int hp, int stamina, int armor, float walkSpeed, float dashSpeed, int attackPower, int actionCooltime, int hearing, int vision, string[] spell, float san)
    {
        _enemyId = enemyId;
        _name = name;
        _hp = hp;
        _stamina = stamina;
        _armor = armor;
        _walkSpeed = walkSpeed;
        _dashSpeed = dashSpeed;
        _attackPower = attackPower;
        _actionCooltime = actionCooltime;
        _hearing = hearing;
        _vision = vision;
        _spell = spell;
        _san = san;
    }
}
