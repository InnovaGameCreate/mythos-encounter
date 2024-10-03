using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class LookDisplayObject : MonoBehaviour
    {
        [SerializeField] private Camera _displayCamera;
        [SerializeField] private GameObject　_uiPanel;
        [SerializeField] private NetworkRunner _runnerPrefab;
        [SerializeField] private float _motionTime = 1f;

        private int _isOpened = 0; //ディスプレイUIの表示状態
        private Vector3 _displayPosition = Vector3.zero; //ディスプレイのTransform
        private Quaternion _displayRotation = Quaternion.identity;

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
        }

        /// <summary>
        /// ディスプレイに注目する
        /// </summary>
        public async void OnEnableDisplay()
        {
            //操作後一回しか通らないように制御
            if (_isOpened != 0) return;
            else _isOpened = 1;

            //モーション前処理
            _displayCamera.enabled = true;

            //処理待機
            await UniTask.WhenAll(
                CameraMotion(Camera.main.transform.position, //カメラモーション
                Camera.main.transform.rotation,
                _displayPosition,
                _displayRotation),
                BootRunner()); //NetworkRunner起動

            //モーション後処理
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _uiPanel.SetActive(true);
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

            //モーション前処理
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _uiPanel.SetActive(false);
            DiscardRunner(); //Runnerの停止

            //処理待機
            await CameraMotion(_displayPosition, _displayRotation, Camera.main.transform.position, Camera.main.transform.rotation);

            //モーション後処理
            _displayCamera.enabled = false;
            _isOpened = 0;
        }

        private async UniTask CameraMotion(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition, Quaternion endRotation)
        {
            float timer = 0f;
            while (timer < _motionTime)
            {
                timer += Time.deltaTime; //タイマー計測
                _displayCamera.transform.position = Vector3.Slerp(startPosition, endPosition, timer / _motionTime); //モーション
                _displayCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, timer / _motionTime);
                await UniTask.Yield(PlayerLoopTiming.Update); //1フレーム待つ
            }
        }

        /// <summary>
        /// NetworkRunnerを起動する
        /// </summary>
        private async UniTask BootRunner()
        {
            var runner = Instantiate(_runnerPrefab); //Runnerインスタンスを設置
            runner.ProvideInput = true; //入植権限を使う

            var result = await runner.JoinSessionLobby(SessionLobby.ClientServer); //セッションロビーに参加（仮想的）
            if (result.Ok)
            {
                //Debug.Log("JoinSessionLobby");
            }
            else
            {
                //Debug.LogError($"Error : {result.ShutdownReason}");
            }
        }

        private void DiscardRunner()
        {
            if (RunnerManager.Runner == null)
            {
                Debug.LogError("Error : Not Found Runner");
            }
            else
            {
                RunnerManager.Runner.Shutdown();
            }
        }
    }
}
