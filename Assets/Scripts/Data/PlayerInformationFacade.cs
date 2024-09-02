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
        public enum spellRequestType
        {
            All,        //すべての呪文所持情報
            Owned,      //持っている呪文だけの所持情報
            NotOwned    //持っていない呪文の所持情報
        }
        public enum EnemyRequestType
        {
            All,        //すべての敵との遭遇数
            Met,        //会ったことがある敵との遭遇数
            NotMet      //会ったことがないある敵との遭遇数＝０
        }

        public static PlayerInformationFacade Instance;
        PlayerInformation playerInformation;
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            playerInformation = PlayerInformation.Instance;
        }

        /// <summary>
        /// 脱出数についての参照
        /// </summary>
        public int GetEscapeCount(EscapeRequestType type = EscapeRequestType.EscapeAndDispersingEscape)
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
        /// 遭遇した敵が初めてか
        /// </summary>
        public bool IsFarstContactEnemy(int id)
        {
            return playerInformation.MythCreature.Where(x => x.Key == id && x.Value == 0).Any();
        }

        /// <summary>
        /// アイテムついての参照
        /// </summary>
        public Dictionary<int, int> GetEnemy(EnemyRequestType type = EnemyRequestType.All)
        {
            switch (type)
            {
                case EnemyRequestType.All:
                    return playerInformation.MythCreature;
                case EnemyRequestType.Met:
                    return playerInformation.MythCreature.Where(x => x.Value > 0).ToDictionary(x => x.Key, x => x.Value);
                case EnemyRequestType.NotMet:
                    return playerInformation.MythCreature.Where(x => x.Value == 0).ToDictionary(x => x.Key, x => x.Value);
                default:
                    Debug.LogError("引数が正確ではありません");
                    return new Dictionary<int, int>();
            }
        }

        /// <summary>
        /// アイテムついての参照
        /// </summary>
        public Dictionary<int, int> GetItem(ItemRequestType type = ItemRequestType.All)
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

        /// <summary>
        /// アイテムついての参照
        /// </summary>
        public Dictionary<int, SpellStruct> GetSpell(spellRequestType type = spellRequestType.All)
        {
            switch (type)
            {
                case spellRequestType.All:
                    return playerInformation.Spell.ToDictionary(x => x.Key, x => WebDataRequest.GetSpellDataArrayList[x.Key]);
                case spellRequestType.Owned:
                    return playerInformation.Spell.Where(x => x.Value == true).ToDictionary(x => x.Key, x => WebDataRequest.GetSpellDataArrayList[x.Key]);
                case spellRequestType.NotOwned:
                    return playerInformation.Spell.Where(x => x.Value == false).ToDictionary(x => x.Key, x => WebDataRequest.GetSpellDataArrayList[x.Key]);
                default:
                    Debug.LogError("?????????m??????????????");
                    return null;
            }
        }

        public void UnLockSpell(int spellId)
        {
            playerInformation.SpellUnlock(spellId);
        }

        public void GetItem(int itemId,int count)
        {
            playerInformation.GetItem(itemId, count);
        }
        public void LostItem(int itemId,int count)
        {
            int lostCount = 0;
            if(playerInformation.Items[itemId] > count)
            {
                lostCount = count;
            }
            else
            {
                lostCount = playerInformation.Items[itemId];
            }
            playerInformation.GetItem(itemId, -lostCount);
        }
        public void MetEnemy(int enemyId)
        {
            playerInformation.MetEnemy(enemyId);
        }
    }
}