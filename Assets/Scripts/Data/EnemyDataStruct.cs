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

    public int enemyId { get => _enemyId; }//�L�����N�^�[ID
    public string name { get => _name; }//�_�b������
    public int hp { get => _hp; }//�̗�
    public int stamina { get => _stamina; }//�X�^�~�i
    public int adArmor { get => _adArmor; }//�����h��
    public int mrArmor { get => _mrArmor; }//���@�h��
    public int walkSpeed { get => _walkSpeed; }//���s���x
    public int dashSpeed { get => _dashSpeed; }//�������x
    public int attackPower { get => _attackPower; }//�U����
    public int actionCooltime { get => _actionCooltime; }//�A�N�V�����N�[���^�C��
    public int hearing { get => _hearing; }//���o
    public int bision { get => _vision; }//���o
    public string[] spell { get => _spell; }//����
    public int spellCount { get => _spellCount; }//�����̊o���Ă��鐔
    public float san { get => _san; }//���F������SAN�l

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
