using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
            _blackImage.DOFade(0, 0.5f);
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
