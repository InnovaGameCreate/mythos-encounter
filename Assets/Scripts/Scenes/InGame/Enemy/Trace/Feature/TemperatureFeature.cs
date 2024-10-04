using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering.HighDefinition;
using UniRx;

namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class TemperatureFeature : FeatureBase
    {
        CancellationTokenSource _cancellationTokenSource;
        FeatureView _view;
        private float _change;
      
        public override void Init(FeatureView view)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _view = view;
            _view.OnFloor.Skip(1).Subscribe(_ =>
            {
                _change = _view.stagetile.Temperature - 10;
                _view.Temperature(_change);
            }).AddTo(view.gameObject); ;
            
        }


    public override void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}