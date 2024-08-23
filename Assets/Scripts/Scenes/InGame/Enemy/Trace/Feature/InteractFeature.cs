using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Threading;
using Scenes.Ingame.Player;

namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class InteractFeature : MonoBehaviour
    {
        CancellationTokenSource _cancellationTokenSource;
        private GameObject[] _stageInteracts;
        private GameObject[] nearStageObject = null;
        private GameObject interactTarget = null;
        private const float INTERACTRANGE = 5;
        void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _stageInteracts = GameObject.FindGameObjectsWithTag("StageIntract");
            InteractLoop(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid InteractLoop(CancellationToken token)
        {
            while(true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(10f), cancellationToken: token);
                nearStageObject = _stageInteracts.Where(target => Vector3.Distance(target.transform.position, this.transform.position) < INTERACTRANGE).ToArray();
                if(nearStageObject.Length > 0)
                {
                    interactTarget = nearStageObject[UnityEngine.Random.Range(0, nearStageObject.Length)];
                    if(interactTarget.TryGetComponent(out IInteractable act))
                    {
                        act.Intract(null);
                    }
                }
            }
        }
        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}