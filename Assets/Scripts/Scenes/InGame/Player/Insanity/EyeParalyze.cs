using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

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

        private Volume _volume;
        private Vignette _vignette;
        public void Setup()
        {
            _volume = FindObjectOfType<Volume>();
            if (!_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette = _volume.profile.Add<Vignette>(false);
            }
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }

            //視野狭める
            _vignette.active = true;//Vignetteの有効化

            //Mapにノイズを走らせる
        }

        public void Hide()
        {
            _vignette.active = false;//Vignetteの無効化
        }
    }
}