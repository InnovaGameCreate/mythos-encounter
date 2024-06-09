using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using Scenes.Ingame.Manager;
using Scenes.Ingame.Player;
using System;

namespace Scenes.Ingame.InGameSystem
{
    public class EscapePoint : MonoBehaviour, IInteractable
    {
        IngameManager manager;
        private bool _isAnimation = false;
        private bool _isActive = false;
        CancellationTokenSource token;
        private bool viewDebugLog = true;//確認用のデバックログを表示する

        private int _escpaeItemCount = 0;
        private int _progress = 0;
        private const string RITUAL = "儀式を開始";
        private const string ESCAPE = "脱出";
        private const int CASTTIME = 3;//詠唱時間
        void Start()
        {
            token = new CancellationTokenSource();
            manager = IngameManager.Instance;
            _isActive = false;
            _escpaeItemCount = manager.GetEscapeItemCount;
            manager.OnOpenEscapePointEvent.Subscribe(_ =>
                {
                    Debug.Log("Active");
                    _isActive = true;
                }).AddTo(this);
            _isActive = true;
        }
        private void OnDestroy()
        {
            token.Cancel();
            token.Dispose();
        }

        public async void Intract(PlayerStatus status)
        {
            if (viewDebugLog) Debug.Log($"EscapeItem.Interact:Button = {Input.GetMouseButtonDown(1)},progress = {_escpaeItemCount >= _progress}, notAnimation = {!_isAnimation}, active = {_isActive}");
            if (Input.GetMouseButtonDown(1) &&
                _escpaeItemCount >= _progress &&
                !_isAnimation &&
                _isActive)
            {
                status.UseEscapePoint(true);
                status.ChangeSpeed();
                await Ritual(token.Token);
                status.UseEscapePoint(false);
                status.ChangeSpeed();
            }
        }

        async UniTask Ritual(CancellationToken token)
        {
            if (viewDebugLog) Debug.Log("StartRitual");
            _isAnimation = true;
            await UniTask.Delay(TimeSpan.FromSeconds(CASTTIME));
            if (_escpaeItemCount > _progress)
            {
                _progress++;
                _isAnimation = false;
                if (viewDebugLog) Debug.Log($"EndRitual, progress {_progress}");
            }
            else
            {
                manager.Escape();
                if (viewDebugLog) Debug.Log("Escape");
            }
        }
        public string ReturnPopString()
        {
            if (_escpaeItemCount >= _progress)
            {
                return RITUAL;
            }
            else
            {
                return ESCAPE;
            }
        }
    }
}