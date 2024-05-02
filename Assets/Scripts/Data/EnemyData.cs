public struct EnemyData
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
    private string _magic;
    private int _magicCount;
    private float _san;

    public int enemyId { get => _enemyId; }
    public string name { get => _name; }
    public int hp { get => _hp; }
    public int stamina { get => _stamina; }
    public int adArmor { get => _adArmor; }
    public int mrArmor { get => _mrArmor; }
    public int walkSpeed { get => _walkSpeed; }
    public int dashSpeed { get => _dashSpeed; }
    public int attackPower { get => _attackPower; }
    public int actionCooltime { get => _actionCooltime; }
    public int hearing { get => _hearing; }
    public int bision { get => _vision; }
    public string magic { get => _magic; }
    public int magicCount { get => _magicCount; }
    public float san { get => _san; }

    public void EnemyDataSet(int enemyId,string name, int hp, int stamina, int adArmor, int mrArmor, int walkSpeed,int dashSpeed, int attackPower,  int hearing, int vision,int actionCooltime, string magic, int magicCount, float san)
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
        _magic = magic;
        _magicCount = magicCount;
        _san = san;
    }
}
