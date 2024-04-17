using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
namespace Scenes.Ingame.Manager
{
    public class EscapeItem : MonoBehaviour
    {
        IngameManager manager;
        private bool _isActive = false;
        private bool _get = false;
        CancellationTokenSource token;
        void Start()
        {
            token = new CancellationTokenSource();
            manager = IngameManager.Instance;
            manager.OnInitial.First().Subscribe(_ => _isActive = false).AddTo(this);
            manager.OnIngame.First().Subscribe(_ =>
            { 
                _isActive = true;
                GetItem(token.Token).Forget();
            }).AddTo(this);

        }
        async UniTaskVoid GetItem(CancellationToken token)
        {
            await UniTask.WaitUntil(() => _get);
            manager.GetEscapeItem();
            Destroy(gameObject, 0.5f);
        }
        private void OnTriggerEnter(Collider collision)
        {
            if (_isActive == false) return;
            if (collision.gameObject.CompareTag("Player"))
            {
                _get = true;
            }

        }
        private void OnDestroy()
        {
            token.Cancel();
            token.Dispose();
        }
    }
}