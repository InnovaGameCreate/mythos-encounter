using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Stage
{
    public class LinqRoomData
    {
        private List<string> _linqData = new List<string>();//隣接してる部屋を格納する
        public List<string> linqData { get => _linqData; }
        private bool _linqed;//SpawnRoomと隣接している場合はtrue
        public bool linqed { get => _linqed; }

        /// <summary>
        /// LinqDataの入力
        /// </summary>
        public void SetLinqRoomData(string room)
        {
            _linqData.Add(room);
        }

        /// <summary>
        /// SpawnRoomと隣接している場合はtrue
        /// </summary>
        public void SetLinqed(bool value)
        {
            _linqed = value;
        }

        /// <summary>
        /// SpawnRoomと隣接している場合はtrue
        /// </summary>
        public void ResetList()
        {
            if(_linqData != null && _linqData.Count > 0)
            {
                _linqData.Clear();
            }
        }
    }
}