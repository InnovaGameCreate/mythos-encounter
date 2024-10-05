using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_2 : MonoBehaviour
    {
        [SerializeField] private Button _publicButton;
        [SerializeField] private Button _privateButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private int _maxPlayer = 4;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        /// <summary>
        /// �Z�b�V�����쐬�i�p�u���b�N�j
        /// </summary>
        public async void CreatePublicSession()
        {
            ButtonControl(false); //�{�^�����b�N

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //�Z�b�V����������
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //�Z�b�V�����J�n�V�[��
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                PlayerCount = _maxPlayer, //�ő�l���̌���
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
            if (result.Ok == false)
            {
                ButtonControl(true); //�{�^�����b�N����
            }
        }

        /// <summary>
        /// �Z�b�V�����쐬�i�v���C�x�[�g�j
        /// </summary>
        /// <param name="sessionName"></param>
        public async void CreatePrivateSession()
        {
            ButtonControl(false); //�{�^�����b�N

            string sessionName = Guid.NewGuid().ToString();
            Debug.Log("Session Name : " + sessionName);

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //�Z�b�V����������
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //�Z�b�V�����J�n�V�[��
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                SessionName = sessionName, //�Z�b�V������
                PlayerCount = _maxPlayer, //�ő�l���̌���
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
            if (result.Ok == false)
            {
                ButtonControl(true); //�{�^�����b�N����
            }
        }

        /// <summary>
        /// �{�^���̃��b�N����
        /// </summary>
        /// <param name="state"></param>
        private void ButtonControl(bool state)
        {
            _publicButton.interactable = state;
            _privateButton.interactable = state;
            _closeButton.interactable = state;
        }
    }
}
