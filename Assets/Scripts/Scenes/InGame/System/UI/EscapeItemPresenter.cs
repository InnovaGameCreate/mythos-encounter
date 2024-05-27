using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Manager;
using TMPro;
using UnityEngine.UI;

namespace Scenes.Ingame.InGameSystem.UI
{

    public class EscapeItemPresenter : MonoBehaviour
    {
        [SerializeField, Tooltip("�E�o�A�C�e������\��")]
        TextMeshProUGUI _socreText;

        void Start()
        {
            IngameManager ingamemanager = IngameManager.Instance;
            ingamemanager.OnEscapeCount.Subscribe(x => {
                _socreText.text = "�E�o�A�C�e����: " + x;
            }).AddTo(this);
        }
    }
}