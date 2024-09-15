using UnityEngine;

namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class FeaturePresenter : MonoBehaviour
    {
        FeatureView _view;
        FeatureModel _model;

        public void AddFeature(TraceType[] features)
        {
            _view = GetComponent<FeatureView>();
            _view.Init();
            _model = new FeatureModel();
            _model.Init(features, _view);
        }
    }
}