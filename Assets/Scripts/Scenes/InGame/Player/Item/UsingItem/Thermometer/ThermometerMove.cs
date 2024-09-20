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
            int floorlayerMask = LayerMask.GetMask("Floor");//床にだけ反応するようにする
            _textMeshPro.text = string.Empty;

            yield return new WaitForSeconds(3f);
            Physics.Raycast(_playerTransform.position, -transform.up, out hit, 10f, floorlayerMask);
            Debug.DrawRay(_playerTransform.position,  -transform.up * 10f, Color.black);
            
            if(hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                float temperature = hit.collider.gameObject.GetComponent<StageTile>().Temperature;//　真下にあるタイルから温度データ取得
                _textMeshPro.text = temperature.ToString();//　温度計に表示される数字を変更
            }
            else
            {
                Debug.Log("タイルを検知できませんでした");
            }

            _playerItem.ChangeCanUseItem(true);//　アイテムを使用できるようにする
            yield break;
        }

    }
}

