using Scenes.Ingame.Manager;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// プレイヤーのスポーン関係を設定するためのスクリプト
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        public static PlayerSpawner Instance;
        [SerializeField] private GameObject[] _myPlayerPrefab;//プレイヤープレハブ
        [SerializeField] private Vector3[] _spawnPosition;//プレイヤーがスポーンする座標

        public readonly int _playerNum = 1;
        // Start is called before the first frame update
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);


            IngameManager.Instance.OnInitial
                .Subscribe(_ => SpawnPlayer());
        }


        private void SpawnPlayer()
        {
            //指定された人数分複製を行う
            for (int i = 0; i < _playerNum; i++)
            {
                Instantiate(_myPlayerPrefab[i], _spawnPosition[i], Quaternion.identity);
            }           
        }
    }
}

