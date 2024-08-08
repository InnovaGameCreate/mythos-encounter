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
            Escape,                     //脱出の数
            DispersingEscape,           //退散させて脱出した数
            EscapeAndDispersingEscape,  //合計の脱出数
        }

        public enum ItemRequestType
        {
            All,        //すべてのアイテム所持情報
            Owned,      //持っているアイテムだけの所持情報
            NotOwned    //持っていないアイテムの所持情報
        }

        public static PlayerInformationFacade Instance;
        PlayerInformation playerInformation;
        private void Awake()
        {
            Instance = this;
            playerInformation = PlayerInformation.Instance;
        }

        /// <summary>
        /// 脱出数についての参照
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
                    Debug.LogError("引数が正確ではありません");
                    return 0;
            }
        }

        /// <summary>
        /// アイテムついての参照
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
                    Debug.LogError("引数が正確ではありません");
                    return new Dictionary<int, int>();
            }
        }
    }
}