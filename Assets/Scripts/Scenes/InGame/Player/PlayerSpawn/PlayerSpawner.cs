using Scenes.Ingame.Manager;
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
        [Header("プレイヤーのスポーン関係")]
        [SerializeField] private GameObject[] _myPlayerPrefab;//プレイヤープレハブ
        [SerializeField] private GameObject[] _spawnPosition;//プレイヤーがスポーンする座標

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
            //指定された人数分複製を行う
            for (int i = 0; i < _playerNum; i++)
            {
                Instantiate(_myPlayerPrefab[i], _spawnPosition[i].transform.position, Quaternion.identity);
            }

            //PlayerUIを１つだけ生成する。
            Instantiate(_playerUI, Vector3.zero ,Quaternion.identity) ;

            //プレイヤーの沸きが完了したことを知らせる
            IngameManager.Instance.SetReady(ReadyEnum.PlayerReady);
        }
    }
}

