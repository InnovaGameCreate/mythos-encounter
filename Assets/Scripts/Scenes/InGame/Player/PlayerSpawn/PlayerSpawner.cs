using Scenes.Ingame.Manager;
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
        [Header("�v���C���[�̃X�|�[���֌W")]
        [SerializeField] private GameObject[] _myPlayerPrefab;//�v���C���[�v���n�u
        [SerializeField] private GameObject[] _spawnPosition;//�v���C���[���X�|�[��������W

        [Header("UI")]
        [SerializeField] private GameObject _playerUI;

        public readonly int _playerNum = 1;
        // Start is called before the first frame update
        void Start()
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
                Instantiate(_myPlayerPrefab[i], _spawnPosition[i].transform.position, Quaternion.identity);
            }

            //PlayerUI���P������������B
            Instantiate(_playerUI, Vector3.zero ,Quaternion.identity) ;

            //�v���C���[�̕����������������Ƃ�m�点��
            IngameManager.Instance.SetReady(ReadyEnum.PlayerReady);
        }
    }
}

