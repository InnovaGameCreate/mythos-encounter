using UnityEngine;

namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class FeaturePresenter : MonoBehaviour
    {
        FeatureView _view;
        FeatureModel _model;
        AudioSource _audioSource;

        public void AddFeature(TraceType[] features)
        {
            _audioSource = GetComponent<AudioSource>();
            _view = GetComponent<FeatureView>();
            _view.Init(_audioSource);
            _model = new FeatureModel();
            _model.Init(features, _view);
        }
    }
}