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
        public enum spellRequestType
        {
            All,        //���ׂĂ̎����������
            Owned,      //�����Ă�����������̏������
            NotOwned    //�����Ă��Ȃ������̏������
        }
        public enum EnemyRequestType
        {
            All,        //���ׂĂ̓G�Ƃ̑�����
            Met,        //��������Ƃ�����G�Ƃ̑�����
            NotMet      //��������Ƃ��Ȃ�����G�Ƃ̑��������O
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
        /// �E�o���ɂ��Ă̎Q��
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
                    Debug.LogError("���������m�ł͂���܂���");
                    return 0;
            }
        }

        /// <summary>
        /// ���������G�����߂Ă�
        /// </summary>
        public bool IsFarstContactEnemy(int id)
        {
            return playerInformation.MythCreature.Where(x => x.Key == id && x.Value == 0).Any();
        }

        /// <summary>
        /// �A�C�e�����Ă̎Q��
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
                    Debug.LogError("���������m�ł͂���܂���");
                    return new Dictionary<int, int>();
            }
        }

        /// <summary>
        /// �A�C�e�����Ă̎Q��
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
                    Debug.LogError("���������m�ł͂���܂���");
                    return new Dictionary<int, int>();
            }
        }

        /// <summary>
        /// �A�C�e�����Ă̎Q��
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