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
    /// ‰æ–Ê‚ÌˆÃ“]‚ğ©İ‚És‚¤‚½‚ß‚ÌƒXƒNƒŠƒvƒg.
    /// å‚É€–S‚É—p‚¢‚é
    /// </summary>
    public class FadeBlackImage : MonoBehaviour
    {
        [SerializeField] private Image _blackImage;
        [SerializeField] private Canvas _thisCanvas;

        public void SubscribeFadePanelEvent()
        {
            IngameManager.Instance.OnPlayerSpawnEvent
                .Subscribe(_ =>
                {
                    PlayerStatus[] _playerStatuses = FindObjectsOfType<PlayerStatus>();
                    foreach (PlayerStatus playerStatus in _playerStatuses)
                    {
                        playerStatus.OnPlayerSurviveChange
                            .Skip(1)
                            .Subscribe(x =>
                            {
                                if (x)//¶‚«•Ô‚Á‚½‚Æ‚«
                                {
                                    FadeOutImage();
                                }
                                else //€‚ñ‚¾‚Æ‚«
                                {
                                    FadeInImage();
                                }
                            }).AddTo(this);
                    }
                    Debug.Log("€–S‚ÌˆÃ“]‚ª‚Å‚«‚é‚æ‚¤‚É‚È‚Á‚½‚æ");
                }).AddTo(this);
        }

        /// <summary>
        /// ‰æ–ÊˆÃ“]
        /// </summary>
        public void FadeInImage()
        {
            _thisCanvas.sortingOrder = 99;
            _blackImage.DOFade(1, 2f).SetDelay(0.5f); 
        }

        /// <summary>
        /// ‰æ–ÊˆÃ“]‚Ì‰ğœ
        /// </summary>
        public void FadeOutImage()
        {
            _thisCanvas.sortingOrder = -1;
            var sequence = DOTween.Sequence(); //Sequence¶¬

            //Tween‚ğ‚Â‚È‚°‚é
            sequence.Append(_blackImage.DOFade(0.3f, 4))
                    .Append(_blackImage.DOFade(0.95f, 3))
                    .Append(_blackImage.DOFade(0f, 5));

            sequence.Play();

        }

        /// <summary>
        /// canvas‚ğ–³Œø‰»
        /// </summary>
        public void DisableCanvas()
        {
            _thisCanvas.enabled = false;
        }
    }
}
