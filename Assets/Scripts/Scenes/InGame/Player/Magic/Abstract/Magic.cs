using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 呪文の雛形となる抽象クラス.
    /// 子クラスの名前は〇〇Magicとすること
    /// 各子クラスは直接playerにアタッチする予定.
    /// </summary>
    public abstract class Magic : MonoBehaviour
    {
        [HideInInspector] public float coolTime;
        [HideInInspector] public int consumeSanValue;

        public void Start()
        {
            ChangeFieldValue();
        }
        /// <summary>
        /// 呪文の効果を実装する関数
        /// </summary>
        public abstract void MagicEffect();

        /// <summary>
        /// 変数を設定する関数
        /// </summary>
        public abstract void ChangeFieldValue();
    }
}
