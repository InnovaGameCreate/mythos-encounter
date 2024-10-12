using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Scenes.Ingame.Stage;

namespace Scenes.Ingame.InGameSystem
{
    /// <summary>
    /// �C�ӂ̃I�u�W�F�N�g��byte�z��ɃV���A���C�Y����
    /// </summary>
    public static class BinarySerializer<T>
    {
        public static byte[] SerializeToBytes(T[,] array)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                var translatedArray = TranslateArrayFromTwoDimention(array);    //1�����z��ɕϊ�
                DataContractSerializer serializer = new DataContractSerializer(typeof(T[]));
                serializer.WriteObject(memoryStream, translatedArray);
                for(int i = 0; i < 12*12; i++)
                {
                    Debug.Log("data:" + memoryStream.ToArray()[i]);
                }
                return memoryStream.ToArray();
            }
        }

        public static byte[] SerializeToBytes(T value)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, ConvertToJson(value));
                return memoryStream.ToArray();
            }
        }

        /*public static T[] DeserializeFromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T[]));
                for (int i = 0; i < 12 * 12; i++)
                {
                    Debug.Log("�����Ƃ����f�[�^:" + memoryStream.ToArray()[i]);
                }
                T[] returnArray = (T[])serializer.ReadObject(memoryStream);
                return (T[])serializer.ReadObject(memoryStream);
            }
        }*/

        public static T DeserializeFromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                string json = (string)serializer.ReadObject(memoryStream);

                StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Json/test_OutPut.json"); //json���t�@�C���ɏ����o��(Debug�p)
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                return ConvertFromJson(json);
            }
        }

        //2�����z���1�����z��ɕϊ�����
        public static T[] TranslateArrayFromTwoDimention(T[,] array)
        {
            T[] translatedArray = new T[array.Length];
            int index = 0;
            for(int i = 0; i < array.GetLength(0); i++)
            {
                for(int j = 0; j < array.GetLength(1); j++)
                {
                    translatedArray[index] = array[i, j];
                }
            }

            return translatedArray;
        }

        //1�����z���cols���Arows���s�Ƃ���2�����z��ɕϊ�
        public static T[,] TranslateTwoDimentionArrayFromArray(T[] array, int cols, int rows)
        {
            T[,] translatedArray = new T[cols, rows];
            int index = 0;
            for(int i = 0; i < cols * rows; i++)
            {
                Debug.Log("�f�V���A���C�Y�O�f�[�^:" + array[i]);
            }
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    translatedArray[i, j] = array[index++];
                    Debug.Log("DeSerialize:" + translatedArray[i, j]);
                }
            }
            return translatedArray;
        }

        public static string ConvertToJson(T data)    //data��json�ɕϊ�����
        {
            string json = JsonUtility.ToJson(data, true);   //json��
            Debug.Log("Json:" + json);
            Debug.Log("��������");
            StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Json/test_Input.json"); //json���t�@�C���ɏ����o��(Debug�p)
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();

            return json;
        }

        public static T ConvertFromJson(string json)
        {
            T data = JsonUtility.FromJson<T>(json); //json���N���X�Ƀf�V���A���C�Y

            return data;
        }
    }
}

