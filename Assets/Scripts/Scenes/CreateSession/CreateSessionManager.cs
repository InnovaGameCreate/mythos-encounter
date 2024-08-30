using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using TMPro;
using Network;
using Prefabs;

namespace Scenes.CreateSession
{
    public class CreateSessionManager : MonoBehaviour
    {
        [SerializeField] private NetworkSystemConfig _networkSystemConfig;
        [Header("SessionNameLength")]
        [SerializeField] private float _sessionNameLengthMin = 1;
        [SerializeField] private float _sessionNameLengthMax = 12;
        [Header("Canvas Objects")]
        [SerializeField] private Transform _canvasTransform;
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_InputField _sessionNameInputField;
        [SerializeField] private Toggle _isPrivateToggle;
        [SerializeField] private Button _createSessionButton;
        [SerializeField] private Dialog _dialogPrefab;

        public async void OnCreateSessionButton()
        {
            //�Z�b�V���������d�����Ă��Ȃ������m�F
            if (RunnerManager.Instance.sessionList.Exists(x => x.Name == _sessionNameInputField.text))
            {
                //Debug.Log("Exist Session Name");

                AllInteractable(false);
                var dialog = Instantiate(_dialogPrefab, _canvasTransform); //�_�C�A���O���o��
                dialog.Init("Exist Session Name", () => { AllInteractable(true); }, null); //���̃_�C�A���O�̏������`����

                return;
            }

            //�Z�b�V�������̕���������
            if (_sessionNameInputField.text.Length < _sessionNameLengthMin
                || _sessionNameInputField.text.Length > _sessionNameLengthMax)
            {
                //Debug.Log("Session Name Length Error");

                AllInteractable(false);
                var dialog = Instantiate(_dialogPrefab, _canvasTransform);
                dialog.Init("Session Name Length Error", () => { AllInteractable(true); }, null);

                return;
            }

            //�{�^����G����Ȃ���Ԃɂ���
            AllInteractable(false);

            //�Z�b�V�����̃J�X�^���v���p�e�B�̍쐬
            var customProps = new Dictionary<string, SessionProperty>();
            customProps["visible"] = !_isPrivateToggle.isOn; //�Z�b�V���������J���邩�̐ݒ�

            //NetworkRunner�ɓn��StartGameArgs�̐ݒ�
            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.Host, //�Z�b�V�������ł̌���
                Scene = SceneRef.FromIndex(_networkSystemConfig.inGameScene), //�J�n�Q�[���V�[���̑I��
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Runner�ł̃V�[���̐���ɕK�v
                SessionName = _sessionNameInputField.text, //�Z�b�V������
                SessionProperties = customProps, //�Z�b�V�����v���p�e�B
                PlayerCount = _networkSystemConfig.sessionPlayerMax, //�ő�l���̌���
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̃g�[�N��(�ꎞ�I)
            };

            //�Z�b�V�����ɎQ��
            var result = await RunnerManager.Instance.JoinSession(startGameArgs);

            //�Z�b�V�����̍쐬�Ɏ��s�����̂Ń{�^�����b�N������
            if (result == false) AllInteractable(true);
        }

        public void OnBackButton()
        {
            SceneManager.LoadScene("SessionLobby");
        }

        /// <summary>
        /// �S�Ă�UI�I�u�W�F�N�g��Interactable�𑀍삷��
        /// </summary>
        /// <param name="state"></param>
        public void AllInteractable(bool state)
        {
            _backButton.interactable = state;
            _sessionNameInputField.interactable = state;
            _isPrivateToggle.interactable = state;
            _createSessionButton.interactable = state;
        }
    }
}
