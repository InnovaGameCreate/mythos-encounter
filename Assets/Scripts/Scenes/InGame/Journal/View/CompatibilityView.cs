using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Enemy.Trace;

public class CompatibilityView : ViewBase
{
    [SerializeField] private GridView _girdView;
    private TraceFeatureController _traceFeatureController;
    public override void Init()
    {
        _traceFeatureController = FindObjectOfType<TraceFeatureController>();
        WebDataRequest.instance.OnEndLoad.Subscribe(_ =>
        {
            _girdView.Init(WebDataRequest.GetEnemyDataArrayList);
            _girdView.UpdateJournalList(_traceFeatureController.traceModel.usedCombinations);
        }).AddTo(this);
        base.Init();
    }
}
