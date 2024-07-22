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
    /// ��ʂ̈Ó]�����݂ɍs�����߂̃X�N���v�g.
    /// ��Ɏ��S���ɗp����
    /// </summary>
    public class FadeBlackImage : MonoBehaviour
    {
        [SerializeField] private Image _blackImage;
        [SerializeField] private Canvas _thisCanvas;

        /// <summary>
        /// ��ʈÓ]
        /// </summary>
        public void FadeInImage()
        {
            _thisCanvas.sortingOrder = 99;
            _blackImage.DOFade(1, 2f).SetDelay(0.5f); 
        }

        /// <summary>
        /// ��ʈÓ]�̉���
        /// </summary>
        public void FadeOutImage()
        {
            _thisCanvas.sortingOrder = -1;
            var sequence = DOTween.Sequence(); //Sequence����

            //Tween���Ȃ���
            sequence.Append(_blackImage.DOFade(0.3f, 4))
                    .Append(_blackImage.DOFade(0.95f, 3))
                    .Append(_blackImage.DOFade(0f, 5));

            sequence.Play();

        }

        /// <summary>
        /// canvas�𖳌���
        /// </summary>
        public void DisableCanvas()
        {
            _thisCanvas.enabled = false;
        }
    }
}
