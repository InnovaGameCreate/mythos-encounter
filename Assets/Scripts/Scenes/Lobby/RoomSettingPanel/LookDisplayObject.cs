using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Fusion;
using DG.Tweening;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class LookDisplayObject : MonoBehaviour
    {
        private enum DisplayState //ディスプレイ制御フラグ
        {
            Close,
            Motion,
            Open,
        }

        [SerializeField] private Camera _displayCamera;
        [SerializeField] private GameObject　_uiPanel;
        [SerializeField] private NetworkRunner _runnerPrefab;
        [SerializeField] private float _motionTime = 1f;

        private DisplayState _displayState = DisplayState.Close; //ディスプレイUIの表示状態
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
            if (_displayState != DisplayState.Close) return;
            else _displayState = DisplayState.Motion;

            //モーション前処理
            _displayCamera.enabled = true;

            //処理待機
            await UniTask.WhenAll(
                CameraMove(Camera.main.transform.position, _displayPosition),
                CameraRotate(Camera.main.transform.rotation, _displayRotation),
                BootRunner()); //NetworkRunner起動

            //モーション後処理
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _uiPanel.SetActive(true);
            _displayState = DisplayState.Open;
        }

        /// <summary>
        /// 視点を戻す
        /// </summary>
        public async void OnDisableDisplay()
        {
            //操作後一回しか通らないように制御
            if (_displayState != DisplayState.Open) return;
            else _displayState = DisplayState.Motion;

            //モーション前処理
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _uiPanel.SetActive(false);
            DiscardRunner(); //Runnerの停止

            //処理待機
            await UniTask.WhenAll(
                CameraMove(_displayPosition, Camera.main.transform.position),
                CameraRotate(_displayRotation, Camera.main.transform.rotation));

            //モーション後処理
            _displayCamera.enabled = false;
            _displayState = DisplayState.Close;
        }

        /// <summary>
        /// カメラ移動
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        private async UniTask CameraMove(Vector3 startPosition, Vector3 endPosition)
        {
            await _displayCamera.transform
                .DOMove(endPosition, _motionTime)
                .From(startPosition)
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion();
        }

        /// <summary>
        /// カメラ回転
        /// </summary>
        /// <param name="startRotation"></param>
        /// <param name="endRotation"></param>
        /// <returns></returns>
        private async UniTask CameraRotate(Quaternion startRotation, Quaternion endRotation)
        {
            await _displayCamera.transform
                .DORotate(endRotation.eulerAngles, _motionTime)
                .From(startRotation.eulerAngles)
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion();
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

        /// <summary>
        /// NetworkRunnerを停止する
        /// </summary>
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
