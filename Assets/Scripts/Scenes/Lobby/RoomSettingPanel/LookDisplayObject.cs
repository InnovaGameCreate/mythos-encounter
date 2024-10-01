using Cysharp.Threading.Tasks;
using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class LookDisplayObject : MonoBehaviour
    {
        [SerializeField] private Camera _displayCamera;
        [SerializeField] private GameObject　_uiPanel;
        [SerializeField] private float _motionTime = 1f;

        private int _isOpened = 0; //ディスプレイUIの表示状態
        private Vector3 _displayPosition; //PrefabのディスプレイのTransformを記録する
        private Quaternion _displayRotation;

        private PlayerMove _playerMove;

        private void Start()
        {
            _displayPosition = _displayCamera.transform.position;
            _displayRotation = _displayCamera.transform.rotation;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnEnableDisplay();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnDisableDisplay();
            }
        }

        /// <summary>
        /// ディスプレイに注目する
        /// </summary>
        public async void OnEnableDisplay()
        {
            //操作後一回しか通らないように制御
            if (_isOpened != 0) return;
            else _isOpened = 1;

            //座標の取得
            Vector3 startPosition = Camera.main.transform.position;
            Quaternion startRotation = Camera.main.transform.rotation;
            Vector3 endPosition = _displayPosition;
            Quaternion endRotation = _displayRotation;

            //ディスプレイカメラ開始
            _displayCamera.enabled = true;

            //モーション中
            float timer = 0f;
            while (timer < _motionTime)
            {
                timer += Time.deltaTime; //タイマー計測
                _displayCamera.transform.position = Vector3.Slerp(startPosition, endPosition, timer / _motionTime); //モーション
                _displayCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, timer / _motionTime);
                await UniTask.Yield(PlayerLoopTiming.Update); //1フレーム待つ
            }

            //ディスプレイUIの表示
            _uiPanel.SetActive(true);

            //Playerの移動を禁ずる
            if (_playerMove == null)
                _playerMove = FindObjectOfType<PlayerMove>();

            _playerMove.MoveControl(false);
            _playerMove.RotationControl(false);


            //ディスプレイに切り替え（制御）
            _isOpened = 2;
        }

        /// <summary>
        /// 視点を戻す
        /// </summary>
        public async void OnDisableDisplay()
        {
            //操作後一回しか通らないように制御
            if (_isOpened != 2) return;
            else _isOpened = 1;

            //座標の取得
            Vector3 startPosition = _displayPosition;
            Quaternion startRotation = _displayRotation;
            Vector3 endPosition = Camera.main.transform.position;
            Quaternion endRotation = Camera.main.transform.rotation;

            //ディスプレイUI非表示
            _uiPanel.SetActive(false);

            //モーション中
            float timer = 0f;
            while (timer < _motionTime)
            {
                timer += Time.deltaTime; //タイマー計測
                _displayCamera.transform.position = Vector3.Slerp(startPosition, endPosition, timer / _motionTime); //モーション
                _displayCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, timer / _motionTime);
                await UniTask.Yield(PlayerLoopTiming.Update); //1フレーム待つ
            }

            //メインカメラに戻す
            _displayCamera.enabled = false;

            //Playerの移動を許す
            _playerMove.MoveControl(true);
            _playerMove.RotationControl(true);

            //ディスプレイUIから戻る（制御）
            _isOpened = 0;
        }
    }
}
