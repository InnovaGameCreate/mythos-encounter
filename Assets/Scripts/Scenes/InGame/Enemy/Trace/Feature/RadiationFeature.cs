using System;
using Cysharp.Threading.Tasks;
using System.Threading;


namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class RadiationFeature : FeatureBase
    {
        CancellationTokenSource _cancellationTokenSource;
        FeatureView _view;
        private int _change;
        

        public override void Init(FeatureView view)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _view = view;
            _view.TemperatureTrace.Subscribe(_ =>
            {
                _change = _view.stagetile.Msv + 10;
                if (_change < 200)
                    _change = 200;
                _view.Msv(_change);
            });
        }

        public override void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}