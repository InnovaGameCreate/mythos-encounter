using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 発狂の種類の1つ
    /// 1.ミニマップにグリッチノイズが走り見えなくなる
    /// 2.視野が狭くなる（PostProcessing）
    /// </summary>
    public class EyeParalyze : MonoBehaviour, IInsanity
    {
        private bool _isFirst = true;//初めて呼び出されたか

        public void Setup()
        { 
        
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }
        }

        public void Hide()
        {
            
        }
    }
}