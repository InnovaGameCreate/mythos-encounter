using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class CompatibilityView : ViewBase
{
    [SerializeField] private GridView _girdView;
    public override void Init()
    {
        WebDataRequest.instance.OnEndLoad.Subscribe(_ =>
        {
            Debug.Log("CompatibilityView.Init");
            _girdView.Init(WebDataRequest.GetEnemyDataArrayList);
        }).AddTo(this);
        base.Init();
    }
}
