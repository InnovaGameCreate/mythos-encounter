using Scenes.Ingame.Manager;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �v���C���[�̃X�|�[���֌W��ݒ肷�邽�߂̃X�N���v�g
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        public static PlayerSpawner Instance;
        [SerializeField] private GameObject[] _myPlayerPrefab;//�v���C���[�v���n�u
        [SerializeField] private Vector3[] _spawnPosition;//�v���C���[���X�|�[��������W

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
            //�w�肳�ꂽ�l�����������s��
            for (int i = 0; i < _playerNum; i++)
            {
                Instantiate(_myPlayerPrefab[i], _spawnPosition[i], Quaternion.identity);
            }           
        }
    }
}

