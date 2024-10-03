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
        [SerializeField] private GameObject�@_uiPanel;
        [SerializeField] private NetworkRunner _runnerPrefab;
        [SerializeField] private float _motionTime = 1f;

        private int _isOpened = 0; //�f�B�X�v���CUI�̕\�����
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
            if (_isOpened != 0) return;
            else _isOpened = 1;

            //���[�V�����O����
            _displayCamera.enabled = true;

            //�����ҋ@
            await UniTask.WhenAll(
                CameraMotion(Camera.main.transform.position, //�J�������[�V����
                Camera.main.transform.rotation,
                _displayPosition,
                _displayRotation),
                BootRunner()); //NetworkRunner�N��

            //���[�V�����㏈��
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _uiPanel.SetActive(true);
            _isOpened = 2;
        }

        /// <summary>
        /// ���_��߂�
        /// </summary>
        public async void OnDisableDisplay()
        {
            //������񂵂��ʂ�Ȃ��悤�ɐ���
            if (_isOpened != 2) return;
            else _isOpened = 1;

            //���[�V�����O����
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _uiPanel.SetActive(false);
            DiscardRunner(); //Runner�̒�~

            //�����ҋ@
            await CameraMotion(_displayPosition, _displayRotation, Camera.main.transform.position, Camera.main.transform.rotation);

            //���[�V�����㏈��
            _displayCamera.enabled = false;
            _isOpened = 0;
        }

        private async UniTask CameraMotion(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition, Quaternion endRotation)
        {
            float timer = 0f;
            while (timer < _motionTime)
            {
                timer += Time.deltaTime; //�^�C�}�[�v��
                _displayCamera.transform.position = Vector3.Slerp(startPosition, endPosition, timer / _motionTime); //���[�V����
                _displayCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, timer / _motionTime);
                await UniTask.Yield(PlayerLoopTiming.Update); //1�t���[���҂�
            }
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
