using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;


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
            _view.OnFloor.Skip(1).Subscribe(_ =>
            {
                _change = _view.stagetile.Msv + 50;
                if (_change > 200)
                    _change = 200;
                _view.Msv(_change);
            }).AddTo(view.gameObject); ;
        }

        public override void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}