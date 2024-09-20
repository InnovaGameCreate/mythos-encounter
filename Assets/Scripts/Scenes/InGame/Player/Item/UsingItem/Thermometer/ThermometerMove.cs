using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using Scenes.Ingame.Stage;
using TMPro;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class ThermometerMove : MonoBehaviour
    {
        [SerializeField] Transform _playerTransform;
        [SerializeField] PlayerItem _playerItem;
        [SerializeField] TextMeshPro _textMeshPro;


        private void Start()
        {
            _textMeshPro.text = string.Empty;
        }
        public IEnumerator MeasureTemperature()
        {
            RaycastHit hit;
            int floorlayerMask = LayerMask.GetMask("Floor");//���ɂ�����������悤�ɂ���
            _textMeshPro.text = string.Empty;

            yield return new WaitForSeconds(3f);
            Physics.Raycast(_playerTransform.position, -transform.up, out hit, 10f, floorlayerMask);
            Debug.DrawRay(_playerTransform.position,  -transform.up * 10f, Color.black);
            
            if(hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                float temperature = hit.collider.gameObject.GetComponent<StageTile>().Temperature;//�@�^���ɂ���^�C�����牷�x�f�[�^�擾
                _textMeshPro.text = temperature.ToString();//�@���x�v�ɕ\������鐔����ύX
            }
            else
            {
                Debug.Log("�^�C�������m�ł��܂���ł���");
            }

            _playerItem.ChangeCanUseItem(true);//�@�A�C�e�����g�p�ł���悤�ɂ���
            yield break;
        }

    }
}

