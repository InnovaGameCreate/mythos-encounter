using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// ドア,アイテムなどの右クリックでアクションを行えることを示すインタフェース
    /// </summary>
    public interface IInteractable
    {
        //やること
        void Intract(PlayerStatus status);

        /// <summary>
        /// ポップに入れる言葉を取得する(主にStageIntractにのみ使う)
        /// </summary>
        /// <returns></returns>
        string ReturnPopString();
    }
}
