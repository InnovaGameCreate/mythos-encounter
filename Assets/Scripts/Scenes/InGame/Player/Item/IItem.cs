namespace Scenes.Ingame.Player
{ 
    /// <summary>
    /// �A�C�e���Ɋւ���C���^�[�t�F�[�X
    /// </summary>
    public interface IItem
    {

        bool isUsed { get; }
        //�A�C�e���g�p���̌��ʂ��L�q
        void UseItem();
    }
}

