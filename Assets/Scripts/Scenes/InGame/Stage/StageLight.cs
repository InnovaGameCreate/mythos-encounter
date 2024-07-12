using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Stage
{
    public class StageLight : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject _lightObject;
        private float _lightStrength;
        private bool _isOn = false;

        void Start()
        {
            _lightStrength = _lightObject.GetComponent<Light>().intensity;
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
            _lightObject.GetComponent<Light>().intensity = _lightStrength;
        }

        private void LightOff()
        {
            _lightObject.GetComponent<Light>().intensity = 0;
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