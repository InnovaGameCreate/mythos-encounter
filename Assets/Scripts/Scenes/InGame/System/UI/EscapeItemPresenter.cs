using UnityEngine;
using UniRx;
using Scenes.Ingame.Manager;
using TMPro;

namespace Scenes.Ingame.InGameSystem.UI
{

    public class EscapeItemPresenter : MonoBehaviour
    {
        [SerializeField, Tooltip("脱出アイテム数を表示")]
        TextMeshProUGUI _socreText;

        void Start()
        {
            IngameManager ingamemanager = IngameManager.Instance;
            ingamemanager.OnEscapeCount.Subscribe(x =>
            {
                _socreText.text = "脱出アイテム数: " + x;
            }).AddTo(this);
        }
    }
}