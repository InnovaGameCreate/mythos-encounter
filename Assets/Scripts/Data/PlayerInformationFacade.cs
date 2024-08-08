using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Data
{
    public class PlayerInformationFacade : MonoBehaviour
    {
        public enum EscapeRequestType
        {
            Escape,                     //�E�o�̐�
            DispersingEscape,           //�ގU�����ĒE�o������
            EscapeAndDispersingEscape,  //���v�̒E�o��
        }

        public enum ItemRequestType
        {
            All,        //���ׂẴA�C�e���������
            Owned,      //�����Ă���A�C�e�������̏������
            NotOwned    //�����Ă��Ȃ��A�C�e���̏������
        }

        public static PlayerInformationFacade Instance;
        PlayerInformation playerInformation;
        private void Awake()
        {
            Instance = this;
            playerInformation = PlayerInformation.Instance;
        }

        /// <summary>
        /// �E�o���ɂ��Ă̎Q��
        /// </summary>
        public int EscapeCount(EscapeRequestType type)
        {
            switch (type)
            {
                case EscapeRequestType.Escape:
                    return playerInformation.Escape;
                case EscapeRequestType.DispersingEscape:
                    return playerInformation.DispersingEscape;
                case EscapeRequestType.EscapeAndDispersingEscape:
                    return playerInformation.Escape + playerInformation.DispersingEscape;
                default:
                    Debug.LogError("���������m�ł͂���܂���");
                    return 0;
            }
        }

        /// <summary>
        /// �A�C�e�����Ă̎Q��
        /// </summary>
        public Dictionary<int, int> ItemCount(ItemRequestType type)
        {
            switch (type)
            {
                case ItemRequestType.All:
                    return playerInformation.Items;
                case ItemRequestType.Owned:
                    return playerInformation.Items.Where(x => x.Value > 0).ToDictionary(x => x.Key,x => x.Value);
                case ItemRequestType.NotOwned:
                    return playerInformation.Items.Where(x => x.Value == 0).ToDictionary(x => x.Key, x => x.Value);
                default:
                    Debug.LogError("���������m�ł͂���܂���");
                    return new Dictionary<int, int>();
            }
        }
    }
}