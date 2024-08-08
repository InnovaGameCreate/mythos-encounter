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
        private string _name;//キャラクター名
        private DateTime _created;//作成日時
        private DateTime _end;//死亡日時
        private int _money;//所持金
        private Dictionary<int, int> _items = new Dictionary<int, int>();//アイテム
        private Dictionary<int, bool> _spell = new Dictionary<int, bool>();//習得した呪文
        private Dictionary<int , int> _mythCreature = new Dictionary<int, int>();//遭遇した敵
        private int _escape;//退散させずに脱出した回数
        private int _dispersingEscape;//退散して脱出した回数
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

        async void Start()
        {
            Instance = this;
            DecodeData();
        }
        private async Task DecodeData()
        {
            await Task.Delay(1000);
            int itemSize = WebDataRequest.GetItemDataArrayList.Count;//アイテムの数
            int spellSize = WebDataRequest.GetSpellDataArrayList.Count;//敵の数
            int mythCreatureSize = WebDataRequest.GetEnemyDataArrayList.Count;//敵の数

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
                _spell.Add(int.Parse(data.Spell[i]), true);
            }
            for (int i = 0; i < data.Items.Length; i++)
            {
                var decode = data.Items[i].Split('=');
                if (decode.Length > 2) Debug.LogError("Failed decode item data");
                _items.Add(int.Parse(decode[0]), int.Parse(decode[1]));
            }
            for (int i = 0; i < data.MythCreature.Length; i++)
            {
                var decode = data.MythCreature[i].Split('=');
                if (decode.Length > 2) Debug.LogError("Failed decode mythCreature data");
                _mythCreature.Add(int.Parse(decode[0]), int.Parse(decode[1]));
            }
            Debug.Log("end");
        }
    }
}