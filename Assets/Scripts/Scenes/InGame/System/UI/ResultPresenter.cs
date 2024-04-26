using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Manager;

namespace Scenes.Ingame.InGameSystem.UI
{
    public class ResultPresenter : MonoBehaviour
    {
        [SerializeField]
        private GameObject _resultCanvas;
        void Start()
        {
            _resultCanvas.SetActive(false);
            IngameManager.Instance.OnResult
                .Subscribe(_ =>
                {
                    _resultCanvas.SetActive(true);
                }).AddTo(this);
        }
    }
}