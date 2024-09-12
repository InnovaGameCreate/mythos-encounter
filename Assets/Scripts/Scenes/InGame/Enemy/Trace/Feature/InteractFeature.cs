using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using System.Threading;

namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class InteractFeature : FeatureBase
    {
        CancellationTokenSource _cancellationTokenSource;
        FeatureView _view;
        private const float INTERVAL = 1f;

        public override void Init(FeatureView view)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _view = view;
            InteractLoop(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid InteractLoop(CancellationToken token)
        {
            while(true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(INTERVAL), cancellationToken: token);
                _view.TryInteract();
            }
        }

        public override void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}