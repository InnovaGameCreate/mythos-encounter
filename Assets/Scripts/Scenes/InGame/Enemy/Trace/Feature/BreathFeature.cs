using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BreathFeature : MonoBehaviour
{
    CancellationTokenSource _cancellationTokenSource;
    AudioSource _audioSource;
    private const float MINTIME = 5;
    private const float MAXTIME = 8;
    void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _audioSource = GetComponent<AudioSource>();
        BreathLoop(_cancellationTokenSource.Token).Forget();
    }

    private async UniTaskVoid BreathLoop(CancellationToken token)
    {
        while (true)
        {
            var interval = UnityEngine.Random.Range(MINTIME, MAXTIME);
            await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: token);
            _audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}
