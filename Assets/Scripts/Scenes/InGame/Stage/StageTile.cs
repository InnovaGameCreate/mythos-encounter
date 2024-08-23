using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Stage
{
    public class StageTile : MonoBehaviour
    {
        [SerializeField] private int _temperature;
        [SerializeField] private int _keep;
        [SerializeField] private int _msv;
        private int _difference;

        void Start()
        {
            GameObject obj = GameObject.Find("StageManager");
            _temperature = obj.GetComponent<StageManager>().StandardTemperature;
            _keep = _temperature;
            StartCoroutine(InMsvChange());
            StartCoroutine(InTemperatureChange());
        }

        //���x���ݒ艷�x�𒴂����ۂ̏���
        private void OverTemperature()
        {
            if (_temperature < _keep)
            {
                _difference = _keep - _temperature;
                _temperature += _difference / 120;
                if (_keep < _temperature)
                    _temperature = _keep;
            }
            else
            {
                _difference = _temperature - _keep;
                _temperature -= _difference / 120;
                if (_temperature < _keep)
                    _temperature = _keep;
            }
        }

        //msv�̕ω��̏���
        IEnumerator InMsvChange()
        {
            while (true)
            {
                if (100 < _msv)
                {
                    _msv -= 1;
                    yield return new WaitForSeconds(1f);
                }
                _msv = Random.Range(90, 101);
                yield return new WaitForSeconds(0.5f);
            }
        }

        //���x�̕ω��̏���
        IEnumerator InTemperatureChange()
        {
            while (true)
            {
                if (_keep == _temperature)
                {
                    if (Random.Range(0, 2) == 0)
                        _temperature += 3;
                    else
                        _temperature -= 3;
                    _keep = _temperature;
                    yield return new WaitForSeconds(10f);
                }
                else
                {
                    OverTemperature();
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        //�O������̕ύX
        public void TemperatureChange(int value)
        {
            _temperature = value;
        }

        public void MsvChange(int value)
        {
            _msv = value;
        }

        //�O������̎Q��
        public int Temperature
        {
            get { return _temperature; } 
        }

        public int Msv
        {
            get { return _msv; } 
        }

    }
}

