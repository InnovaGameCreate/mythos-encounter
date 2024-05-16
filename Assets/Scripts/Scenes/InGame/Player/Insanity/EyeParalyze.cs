using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 発狂の種類の1つ
    /// 1.ミニマップにグリッチノイズが走り見えなくなる
    /// 2.プレイヤーの画面には外周に赤いモヤを発生させる(出現時と消失時にはゆっくりフェードする)
    /// 3.一部麻痺がおこる
    /// </summary>
    public class EyeParalyze : MonoBehaviour, IInsanity
    {
        public void Setup()
        { 
        
        }

        public void Active()
        { 
        
        }

        public void Hide()
        {
            Destroy(this);
        }
    }
}