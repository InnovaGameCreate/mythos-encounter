namespace Scenes.Ingame.Player
{ 
    /// <summary>
    /// アイテムに関するインターフェース
    /// </summary>
    public interface IItem
    {

        bool isUsed { get; }
        //アイテム使用時の効果を記述
        void UseItem();
    }
}

