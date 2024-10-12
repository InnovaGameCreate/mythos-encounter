using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scenes.Ingame.InGameSystem;
using System.Runtime.Serialization;

namespace Scenes.Ingame.Stage
{
    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// �ʐM����ɃX�e�[�W�Ɋ֘A����f�[�^�𑗐M����p�P�b�g�N���X
    /// </summary>
    [Serializable]
    public class RoomDataArray
    {
        public RoomData[] roomDataArray;
        //public RoomData[] _secondRoomDataArray;
        //public List<SerializableKeyValuePair<int, LinqRoomData>> linqData = new();
        public int _roomId = 0;
        public RoomDataArray(int length, int id, Dictionary<int, LinqRoomData> list)
        {
            //ConvertDicToList(list); //Dictionary��List�ɕϊ�����
            roomDataArray = new RoomData[length];
            //_firstRoomDataArray = new RoomData[_firstLength];
            //_secondRoomDataArray = new RoomData[_secondLength];
            _roomId = id;
        }

        public void SetData(RoomData[,] data)   //JsonUtility��Photon�̓s����1�����z��ɕϊ�
        {
            int index = 0;
            for(int i = 0; i < data.GetLength(0); i++)
            {
                for(int j = 0; j < data.GetLength(1); j++)
                {
                    roomDataArray[index] = data[i, j];
                    //Debug.Log(roomDataArray[index].RoomId);
                    index++;
                }
            }
        }

        /*public void SetSecondData(RoomData[,] data)   //JsonUtility��Photon�̓s����1�����z��ɕϊ�
        {
            int index = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    _secondRoomDataArray[index] = data[i, j];
                    //Debug.Log(roomDataArray[index].RoomId);
                    index++;
                }
            }
        }*/

        public RoomData[,] TranslateTwoDimentionArrayFromArray(int cols, int rows)
        {
            RoomData[,] translatedArray = new RoomData[cols, rows];
            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    translatedArray[i, j] = roomDataArray[index++];
                    Debug.Log("DeSerialize:" + translatedArray[i, j]);
                }
            }
            return translatedArray;
        }

        /*public Dictionary<int, LinqRoomData> ConvertListToDic()
        {
            Dictionary<int, LinqRoomData> data = new Dictionary<int, LinqRoomData>();
            foreach(var pair in linqData)
            {
                data[pair.Key] = pair.Value;
            }

            return data;
        }*/

        /*void ConvertDicToList(Dictionary<int, LinqRoomData> list)
        {
            linqData.Clear();
            using var e = GetEnumerator();
            while (e.MoveNext())
            {
                linqData.Add(new SerializableKeyValuePair<int, LinqRoomData>(e.Current.Key, e.Current.Value));
            }
        }*/
    }
}

