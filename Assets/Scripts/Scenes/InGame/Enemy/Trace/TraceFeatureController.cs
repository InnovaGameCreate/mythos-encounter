using Scenes.Ingame.Manager;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Enemy.Trace.Feature;

namespace Scenes.Ingame.Enemy.Trace
{
    public class TraceFeatureController : MonoBehaviour
    {
        GameObject _enemy;//TODO:敵が複数体だった場合の対応
        FeaturePresenter _featurePresenter;
        [SerializeField]
        private bool _debugAddTrace = false;
        [SerializeField]
        private TraceType[] _deugType;
        void Start()
        {
            _featurePresenter = GetComponent<FeaturePresenter>();
            IngameManager.Instance.OnIngame
                .Subscribe(_ =>
                {
                    _enemy = GameObject.FindWithTag("Enemy");
                    //TODO:敵から痕跡の情報をとって、生成する機能を追加する 
                    AddTrace(_deugType);
                }).AddTo(this);
        }

        private void AddTrace(TraceType[] _deugType)
        {
            _featurePresenter.Init(_deugType);
        }
    }
}
