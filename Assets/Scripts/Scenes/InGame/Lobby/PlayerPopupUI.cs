using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Scenes.Ingame.PlayerUI
{
    /// <summary>
    /// �A�C�e��UI�Ɋւ��鏈�����܂Ƃ߂��N���X
    /// UI���|�b�v�A�b�v�\��
    /// </summary>
    public class PlayerPopupUI : MonoBehaviour
    {
        //UI�֌W
        [SerializeField] GameObject _itemUI;
        //Ray�֘A
        [SerializeField] Camera _mainCamera;//player�̖ڐ���S���J����
        [SerializeField] private float _getItemRange;//�A�C�e�������ł��鋗��

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1)) // ���N���b�N
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, _getItemRange))
                {
                    if (hit.collider.CompareTag("Loadout"))
                    {
                        //��\�����\��
                        _itemUI.SetActive(true);
                    }
                }
            }
        }
    }
}
