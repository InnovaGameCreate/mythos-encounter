using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scenes.Ingame.Manager;

using UniRx;
public class GameContentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentFlee;
    [SerializeField] private TextMeshProUGUI contentColl;
    [SerializeField] private TextMeshProUGUI contentFind;
    [SerializeField] private TextMeshProUGUI contentEsc;
    IngameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = IngameManager.Instance;
        contentFlee.text = "敵神話生物から逃げよう";
        contentColl.text = "脱出アイテムを集めよう";
        contentFind.text = "脱出地点を探そう";
        contentEsc.text = "脱出地点から脱出しよう";

        //脱出アイテムが集まったら線を引く
        manager.OnOpenEscapePointEvent.Subscribe(_ =>
        {
            contentColl.text = "<s>脱出アイテムを集めよう</s>";
        }).AddTo(this);

    }
}
