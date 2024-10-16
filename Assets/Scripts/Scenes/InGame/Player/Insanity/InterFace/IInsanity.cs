using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    public interface IInsanity
    {
        /// <summary>
        /// 初期化処理
        /// </summary>
        void Setup();

        /// <summary>
        /// 効果の適応
        /// </summary>
        void Active();

        /// <summary>
        /// 効果の無効化
        /// </summary>
        void Hide();
    }
}