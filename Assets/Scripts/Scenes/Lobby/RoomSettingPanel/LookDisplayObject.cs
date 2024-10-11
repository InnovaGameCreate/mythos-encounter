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
        private enum DisplayState //�f�B�X�v���C����t���O
        {
            Close,
            Motion,
            Open,
        }

        [SerializeField] private Camera _displayCamera;
        [SerializeField] private GameObject�@_uiPanel;
        [SerializeField] private NetworkRunner _runnerPrefab;
        [SerializeField] private float _motionTime = 1f;

        private DisplayState _displayState = DisplayState.Close; //�f�B�X�v���CUI�̕\�����
        private Vector3 _displayPosition = Vector3.zero; //�f�B�X�v���C��Transform
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
        /// �f�B�X�v���C�ɒ��ڂ���
        /// </summary>
        public async void OnEnableDisplay()
        {
            //������񂵂��ʂ�Ȃ��悤�ɐ���
            if (_displayState != DisplayState.Close) return;
            else _displayState = DisplayState.Motion;

            //���[�V�����O����
            _displayCamera.enabled = true;

            //�����ҋ@
            await UniTask.WhenAll(
                CameraMove(Camera.main.transform.position, _displayPosition),
                CameraRotate(Camera.main.transform.rotation, _displayRotation),
                BootRunner()); //NetworkRunner�N��

            //���[�V�����㏈��
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _uiPanel.SetActive(true);
            _displayState = DisplayState.Open;
        }

        /// <summary>
        /// ���_��߂�
        /// </summary>
        public async void OnDisableDisplay()
        {
            //������񂵂��ʂ�Ȃ��悤�ɐ���
            if (_displayState != DisplayState.Open) return;
            else _displayState = DisplayState.Motion;

            //���[�V�����O����
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _uiPanel.SetActive(false);
            DiscardRunner(); //Runner�̒�~

            //�����ҋ@
            await UniTask.WhenAll(
                CameraMove(_displayPosition, Camera.main.transform.position),
                CameraRotate(_displayRotation, Camera.main.transform.rotation));

            //���[�V�����㏈��
            _displayCamera.enabled = false;
            _displayState = DisplayState.Close;
        }

        /// <summary>
        /// �J�����ړ�
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
        /// �J������]
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
        /// NetworkRunner���N������
        /// </summary>
        private async UniTask BootRunner()
        {
            var runner = Instantiate(_runnerPrefab); //Runner�C���X�^���X��ݒu
            runner.ProvideInput = true; //���A�������g��

            var result = await runner.JoinSessionLobby(SessionLobby.ClientServer); //�Z�b�V�������r�[�ɎQ���i���z�I�j
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
        /// NetworkRunner���~����
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
