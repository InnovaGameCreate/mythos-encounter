using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Security.Cryptography;

public class ReadyFlagManager : NetworkObject
{ 
    public NetworkDictionary<PlayerRef, int> ReadyState = new NetworkDictionary<PlayerRef, int>();
    public int PlayerNum;
    Subject<Unit> _ready = new Subject<Unit>();
    public ISubject<Unit> OnReady => _ready;

    async void Start()
    {
        await UniTask.WaitUntil(() => ReadyState.Count == PlayerNum);
        _ready.OnNext(default);
    }
    public async UniTask WaitClientsReady()
    {
        
    }
}
