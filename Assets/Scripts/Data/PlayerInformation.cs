using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Scenes.Ingame.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Data
{
    public class PlayerInformation : MonoBehaviour
    {
        private int characterId = 1;
        private string _name;//�L�����N�^�[��
        private DateTime _created;//�쐬����
        private DateTime _end;//���S����
        private int _money;//������
        private Dictionary<int, int> _items = new Dictionary<int, int>();//�A�C�e��
        private Dictionary<int, bool> _spell = new Dictionary<int, bool>();//�K����������
        private Dictionary<int , int> _mythCreature = new Dictionary<int, int>();//���������G
        private int _escape;//�ގU�������ɒE�o������
        private int _dispersingEscape;//�ގU���ĒE�o������
        public static PlayerInformation Instance;

        public string Name { get => _name; }
        public DateTime Created { get => _created; }
        public DateTime End { get => _end; }
        public int Money { get => _money; }
        public Dictionary<int, int> Items { get => _items; }
        public Dictionary<int, bool> Spell { get => _spell; }
        public Dictionary<int, int> MythCreature { get => _mythCreature; }
        public int Escape { get => _escape; }
        public int DispersingEscape { get => _dispersingEscape; }

        public void SpellUnlock(int i)
        {
            _spell[i] = true;
        }

        async void Start()
        {
            Instance = this;
            DecodeData();
        }
        public void MetEnemy(int enemyId)
        {
            _mythCreature[enemyId]++;
        }

        public void GetItem(int itemId, int count)
        {
            _items[itemId] += count;
        }
        private async UniTaskVoid DecodeData()
        {
            await UniTask.WaitUntil(() => WebDataRequest.OnCompleteLoadData);
            await Task.Delay(100);
            int itemSize = WebDataRequest.GetItemDataArrayList.Count;//?A?C?e??????
            int spellSize = WebDataRequest.GetSpellDataArrayList.Count;//?G????
            int mythCreatureSize = WebDataRequest.GetEnemyDataArrayList.Count;//?G????

            for (int i = 0; i < itemSize; i++)
            {
                _items[i] = 0;
            }
            for (int i = 0; i < spellSize; i++)
            {
                _spell[i] = false;
            }
            for (int i = 0; i < mythCreatureSize; i++)
            {
                _mythCreature[i] = 0;
            }
            var data = WebDataRequest.GetPlayerDataArrayList.Where(data => data.Id == characterId).FirstOrDefault();
            for (int i = 0; i < data.Spell.Length; i++)
            {
                _spell[int.Parse(data.Spell[i])] =  true;
            }
            for (int i = 0; i < data.Items.Length; i++)
            {
                var decode = data.Items[i].Split('=');
                if (decode.Length > 2) Debug.LogError("Failed decode item data");
                _items[int.Parse(decode[0])] = int.Parse(decode[1]);
            }
            for (int i = 0; i < data.MythCreature.Length; i++)
            {
                var decode = data.MythCreature[i].Split('=');
                if (decode.Length > 2) Debug.LogError($"Failed decode mythCreature data. {data.MythCreature[i]}");
                 Debug.Log($"decode  {data.MythCreature[i]}");
                _mythCreature[int.Parse(decode[0])] = int.Parse(decode[1]);
            }
            Debug.Log("end");
        }
    }
}