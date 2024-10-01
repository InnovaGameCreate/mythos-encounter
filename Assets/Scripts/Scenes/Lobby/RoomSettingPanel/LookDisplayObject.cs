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
        [SerializeField] private GameObject�@_uiPanel;
        [SerializeField] private float _motionTime = 1f;

        private int _isOpened = 0; //�f�B�X�v���CUI�̕\�����
        private Vector3 _displayPosition; //Prefab�̃f�B�X�v���C��Transform���L�^����
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
        /// �f�B�X�v���C�ɒ��ڂ���
        /// </summary>
        public async void OnEnableDisplay()
        {
            //������񂵂��ʂ�Ȃ��悤�ɐ���
            if (_isOpened != 0) return;
            else _isOpened = 1;

            //���W�̎擾
            Vector3 startPosition = Camera.main.transform.position;
            Quaternion startRotation = Camera.main.transform.rotation;
            Vector3 endPosition = _displayPosition;
            Quaternion endRotation = _displayRotation;

            //�f�B�X�v���C�J�����J�n
            _displayCamera.enabled = true;

            //���[�V������
            float timer = 0f;
            while (timer < _motionTime)
            {
                timer += Time.deltaTime; //�^�C�}�[�v��
                _displayCamera.transform.position = Vector3.Slerp(startPosition, endPosition, timer / _motionTime); //���[�V����
                _displayCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, timer / _motionTime);
                await UniTask.Yield(PlayerLoopTiming.Update); //1�t���[���҂�
            }

            //�f�B�X�v���CUI�̕\��
            _uiPanel.SetActive(true);

            //Player�̈ړ����ւ���
            if (_playerMove == null)
                _playerMove = FindObjectOfType<PlayerMove>();

            _playerMove.MoveControl(false);
            _playerMove.RotationControl(false);


            //�f�B�X�v���C�ɐ؂�ւ��i����j
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

            //���W�̎擾
            Vector3 startPosition = _displayPosition;
            Quaternion startRotation = _displayRotation;
            Vector3 endPosition = Camera.main.transform.position;
            Quaternion endRotation = Camera.main.transform.rotation;

            //�f�B�X�v���CUI��\��
            _uiPanel.SetActive(false);

            //���[�V������
            float timer = 0f;
            while (timer < _motionTime)
            {
                timer += Time.deltaTime; //�^�C�}�[�v��
                _displayCamera.transform.position = Vector3.Slerp(startPosition, endPosition, timer / _motionTime); //���[�V����
                _displayCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, timer / _motionTime);
                await UniTask.Yield(PlayerLoopTiming.Update); //1�t���[���҂�
            }

            //���C���J�����ɖ߂�
            _displayCamera.enabled = false;

            //Player�̈ړ�������
            _playerMove.MoveControl(true);
            _playerMove.RotationControl(true);

            //�f�B�X�v���CUI����߂�i����j
            _isOpened = 0;
        }
    }
}
