using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Scenes.Ingame.Manager;
using Scenes.Ingame.Player;
using UniRx;

namespace Scenes.Ingame.InGameSystem.UI
{
    /// <summary>
    /// 画面の暗転を自在に行うためのスクリプト.
    /// 主に死亡時に用いる
    /// </summary>
    public class FadeBlackImage : MonoBehaviour
    {
        [SerializeField] private Image _blackImage;
        [SerializeField] private Canvas _thisCanvas;

        /// <summary>
        /// 画面暗転
        /// </summary>
        public void FadeInImage()
        {
            _thisCanvas.sortingOrder = 99;
            _blackImage.DOFade(1, 2f).SetDelay(0.5f); 
        }

        /// <summary>
        /// 画面暗転の解除
        /// </summary>
        public void FadeOutImage()
        {
            _thisCanvas.sortingOrder = -1;
            var sequence = DOTween.Sequence(); //Sequence生成

            //Tweenをつなげる
            sequence.Append(_blackImage.DOFade(0.3f, 4))
                    .Append(_blackImage.DOFade(0.95f, 3))
                    .Append(_blackImage.DOFade(0f, 5));

            sequence.Play();

        }

        /// <summary>
        /// canvasを無効化
        /// </summary>
        public void DisableCanvas()
        {
            _thisCanvas.enabled = false;
        }
    }
}
