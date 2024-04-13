/// <summary>
/// IngameManageの_initialEvent発行後の_ingameEventが発行されるまでに必要な準備作業
/// </summary>
namespace Scenes.Ingame.Manager
{
    public enum ReadyEnum
    {
        StageReady,
        PlayerReady,
        EnemyReady,
    }
}