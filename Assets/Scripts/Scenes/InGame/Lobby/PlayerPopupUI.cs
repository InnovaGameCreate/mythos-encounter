using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Scenes.Ingame.PlayerUI
{
    /// <summary>
    /// アイテムUIに関する処理をまとめたクラス
    /// UIをポップアップ表示
    /// </summary>
    public class PlayerPopupUI : MonoBehaviour
    {
        //UI関係
        [SerializeField] GameObject _itemUI;
        //Ray関連
        [SerializeField] Camera _mainCamera;//playerの目線を担うカメラ
        [SerializeField] private float _getItemRange;//アイテムを入手できる距離

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1)) // 左クリック
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, _getItemRange))
                {
                    if (hit.collider.CompareTag("Loadout"))
                    {
                        //非表示→表示
                        _itemUI.SetActive(true);
                    }
                }
            }
        }
    }
}
