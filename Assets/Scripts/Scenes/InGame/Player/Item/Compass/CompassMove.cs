using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class CompassMove : MonoBehaviour
{
    private GameObject _escapePoint;
    private GameObject[] _Enemys;
    private float distance;
    private bool _isEnabled = true;
    [Header("j‚Ì‰ñ“]‘¬“x")]
    [SerializeField] private float _rotateSpeed;
    [Header("j‚Ì‰ñ“]üŠú(ŒÌá)")]
    [SerializeField] private float _brokeRotateCycle;
    [Header("ƒRƒ“ƒpƒX‚ª‰ó‚ê‚é‹——£")]
    [SerializeField] private float _unusableDistance;
    private void Start()
    {
        _escapePoint = GameObject.Find("escapePoint(Clone)");
        _Enemys = GameObject.FindGameObjectsWithTag("Enemy");
    }
    void Update()
    {

        for (int i = 0; i < _Enemys.Length; i++)
        {
            Vector3 vector = _Enemys[i].transform.position - this.transform.position;


            if (i == 0)
            {
                distance = vector.magnitude;
            }

            else
            {
                if (vector.magnitude < distance)
                {
                    distance = vector.magnitude;
                }
            }

        }


        if (distance >= _unusableDistance)
        {
            if (_escapePoint)
            {
                if (!_isEnabled)
                {
                    this.transform.DOPause();
                    _isEnabled = true;
                }

                var _targetDirection = _escapePoint.transform.position - this.transform.position;
                _targetDirection.y = 0;

                var lookRotation = Quaternion.LookRotation(_targetDirection, Vector3.up);
                transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, Time.deltaTime * _rotateSpeed);

                Vector3 currentRotation = transform.localEulerAngles;
                transform.localRotation = Quaternion.Euler(0, currentRotation.y, 0);
            }
        }
        else
        {
            if (_isEnabled)
            {
                BrokeRotate();
                _isEnabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"“G‚Æ‚Ì‹——£‚Í{distance}");
            Debug.Log($"ƒRƒ“ƒpƒX‚ÌŠp“x{this.transform.localEulerAngles.y}");

        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            _Enemys = GameObject.FindGameObjectsWithTag("Enemy");

        }
    }

    void BrokeRotate()
    {
        this.transform.DOLocalRotate(new Vector3(0, this.transform.localEulerAngles.y + 360f, 0), _brokeRotateCycle, RotateMode.FastBeyond360)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }
}