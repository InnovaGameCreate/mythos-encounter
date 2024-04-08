namespace Scenes.Ingame.Manager
{
    public struct IngameReady
    {
        public bool StageReady;
        public bool PlayerReady;
        public bool EnemyReady;
        public void Initialize()
        {
            StageReady = false;
            PlayerReady = false;
            EnemyReady = false;
        }

        /// <summary>
        /// �������I�������ꍇ�l���Z�b�g
        /// </summary>
        /// <param name="stage">�X�e�[�W�̐��������������ꍇ</param>
        /// <param name="player">�v���C���[�̐��������������ꍇ</param>
        /// <param name="enemy">�G�̐��������������ꍇ</param>
        public void SetReady(bool stage = false,bool player = false,bool enemy = false)
        {
            if (stage) StageReady = true;
            else if (player) PlayerReady = true;
            else if (enemy) EnemyReady = true;
        }
        public bool Ready()
        {
            if (StageReady && PlayerReady && EnemyReady) return true;
            else return false;
        }
    }
}