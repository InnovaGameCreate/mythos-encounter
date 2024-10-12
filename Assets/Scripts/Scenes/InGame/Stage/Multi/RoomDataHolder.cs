using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using Cysharp.Threading.Tasks;
using System;

namespace Scenes.Ingame.Stage
{
    public class ReliableData
    {
        public int[] keys = new int[4];
        public ArraySegment<byte> data;
    }
    public static class RoomDataHolder
    {
        public static ReliableData _first = new ReliableData();
        public static ReliableData _second = new ReliableData();
        static int count = 0;
        public static bool GetFlag = false;
        public static Multi_StageGenerator StageGenerator = null;

        public static void AddListener(NetworkEvents events)
        {
            events.OnReliableData.AddListener(OnDataRecieve);
        }

        static void OnDataRecieve(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            if(count == 0)
            {
                key.GetInts(out _first.keys[0], out _first.keys[1], out _first.keys[2], out _first.keys[3]);
                _first.data = data;
                count++;
            }
            else
            {
                key.GetInts(out _second.keys[0], out _second.keys[1], out _second.keys[2], out _second.keys[3]);
                _second.data = data;
                count++;
            }
            if(count == 2)
            {
                GetFlag = true;
            }

        }
    }
}

