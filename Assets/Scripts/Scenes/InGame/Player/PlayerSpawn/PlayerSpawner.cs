using Scenes.Ingame.InGameSystem;
using Scenes.Ingame.Manager;
using Scenes.Ingame.Stage;
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

        [Header("UI")]
        [SerializeField] private GameObject _playerUI;

        private Vector3 _spawnPosition;
        private StageGenerator _stageGenerator;
        public readonly int _playerNum = 1;
        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);

            _stageGenerator = GameObject.FindObjectOfType<StageGenerator>();
            _spawnPosition = _stageGenerator.spawnPosition;

            IngameManager.Instance.OnInitial
                .Subscribe(_ => SpawnPlayer());
        }


        private void SpawnPlayer()
        {
            //指定された人数分複製を行う
            for (int i = 0; i < _playerNum; i++)
            {
                Instantiate(_myPlayerPrefab[i], _spawnPosition, Quaternion.identity);
            }

            //PlayerUIを１つだけ生成する。
            Instantiate(_playerUI, Vector3.zero ,Quaternion.identity) ;

            //プレイヤーの沸きが完了したことを知らせる
            IngameManager.Instance.SetReady(ReadyEnum.PlayerReady);
        }
    }
}

