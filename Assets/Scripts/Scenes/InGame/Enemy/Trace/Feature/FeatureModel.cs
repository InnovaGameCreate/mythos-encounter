using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class FeatureModel
    {
        FeatureBase[] _features = new FeatureBase[3];
        public void Init(TraceType[] features, FeatureView view)
        {
            for (int i = 0; i < features.Length; i++)
            {
                switch (features[i])
                {
                    case TraceType.Interact:
                        _features[i] = new InteractFeature();
                        break;
                    case TraceType.Breath:
                        _features[i] = new BreathFeature();
                        break;
                    case TraceType.Growl:
                        _features[i] = new GrowlFeature();
                        break;
                    case TraceType.Appetitte:
                        break;
                    case TraceType.Radiation:
                        break;
                    case TraceType.Temperature:
                        break;
                    case TraceType.Track:
                        break;
                    default:
                        break;
                }
                _features[i].Init(view);
            }
        }
    }
}