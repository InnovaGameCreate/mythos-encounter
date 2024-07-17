using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Stage
{
    public class StageLight : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<GameObject> _lightObject = new List<GameObject>();
        private float[] _lightStrength = new float[10];
        private bool _isOn = false;

        void Awake()
        {
            for (int i = 0; i < _lightObject.Count; i++)
            {
                _lightStrength[i] = _lightObject[i].GetComponent<Light>().intensity;
            }
        }
        public void Intract(PlayerStatus status)
        {
            if (Input.GetMouseButtonDown(1))
            {
                switch (_isOn)
                {
                    case true:
                        LightOff();
                        _isOn = false;
                        break;
                    case false:
                        LightOn();
                        _isOn = true;
                        break;
                }
            }
        }

        private void LightOn()
        {
            for (int i = 0; i < _lightObject.Count; i++)
            {
                _lightObject[i].GetComponent<Light>().intensity = _lightStrength[i];

            }
        }

        private void LightOff()
        {
            for (int i = 0; i < _lightObject.Count; i++)
            {
                _lightObject[i].GetComponent<Light>().intensity = 0;
            } 
        }

        public string ReturnPopString()
        {
            if (_isOn)
                return "Off";
            else
                return "On";
        }
    }
}