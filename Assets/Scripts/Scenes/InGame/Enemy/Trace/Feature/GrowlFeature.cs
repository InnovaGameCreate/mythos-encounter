using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GrowlFeature : MonoBehaviour
{
    CancellationTokenSource _cancellationTokenSource;
    AudioSource _audioSource;
    private const float MINTIME = 10;
    private const float MAXTIME = 20;
    void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _audioSource = GetComponent<AudioSource>();
        GrowlLoop(_cancellationTokenSource.Token).Forget();
    }

    private async UniTaskVoid GrowlLoop(CancellationToken token)
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
