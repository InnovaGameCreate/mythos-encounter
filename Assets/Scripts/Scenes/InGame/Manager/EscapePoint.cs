using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Scenes.Ingame.Manager
{
    public class EscapePoint : MonoBehaviour
    {
        [SerializeField]
        private Material _activeMaterial;
        IngameManager manager;
        private bool _isActive = false;
        private bool _get = false;
        CancellationTokenSource token;
        void Start()
        {
            token = new CancellationTokenSource();
            manager = IngameManager.Instance;
            manager.OnInitial.Subscribe(_ => _isActive = false).AddTo(this);
            manager.OnIngame.Subscribe(_ => _isActive = true).AddTo(this);
            manager.OnOpenEscapePointEvent.Subscribe(_ =>
            {
                GetComponent<Renderer>().material = _activeMaterial;
                GetItem(token.Token).Forget();
            }).AddTo(this);
        }
        async UniTaskVoid GetItem(CancellationToken token)
        {
            await UniTask.WaitUntil(() => _get);
            manager.Escape();
            Destroy(gameObject, 0.5f);
        }
        private void OnTriggerEnter(Collider collision)
        {
            if (_isActive == false) return;
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