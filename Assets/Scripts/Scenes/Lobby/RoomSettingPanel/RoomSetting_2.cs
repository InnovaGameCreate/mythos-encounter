using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_2 : MonoBehaviour
    {
        private static readonly int SESSION_NUMBER_MAX = 999999;
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

            string sessionId = GetSessionId(); //�Z�b�V����ID�̎擾

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //�Z�b�V����������
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //�Z�b�V�����J�n�V�[��
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                SessionName = sessionId, //�Z�b�V������
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

        /// <summary>
        /// �v���C�x�[�g�̃Z�b�V����ID�̎擾
        /// </summary>
        /// <returns></returns>
        private string GetSessionId()
        {
            string sessionId = "";

            while (true)
            {
                //ID����
                int randomNumber = UnityEngine.Random.Range(0, SESSION_NUMBER_MAX);
                string sessionIdTmp = randomNumber.ToString("D6");

                //�d���m�F
                var result = RunnerManager.Instance.SessionInfoList.FirstOrDefault(x => x.Name == sessionIdTmp);
                if (result == null)
                {
                    sessionId = sessionIdTmp;
                    break;
                }
            }

            Debug.LogError("Session Id : " + sessionId);
            return sessionId;
        }
    }
}
