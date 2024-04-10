namespace Scenes.Ingame.Player
{
    
    /// <summary>
    /// プレイヤーの行動を示す
    /// アニメーションの判定等に使う.
    /// </summary>
    public enum PlayerAction
    {
        Idle,//待機状態
        Waik,//歩行
        Sneak,//忍び歩き
        Dash,//走る
        Jump,//ジャンプ
        Item,//アイテム使用時の状態
        Magic,//呪文使用時の状態
    }
}

