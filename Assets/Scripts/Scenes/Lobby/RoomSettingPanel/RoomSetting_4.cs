using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using Common.Network;
using Common.UI;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_4 : MonoBehaviour
    {
        [SerializeField] private Button _enterRoomButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_InputField _sessionName;
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private Dialog _dialog;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        public async void JoinPrivateSession()
        {
            //�Z�b�V���������݂��邩�𔻒�
            string sessionName = _sessionName.text;
            var sessionInfo = RunnerManager.Instance.SessionInfoList.FirstOrDefault(x => x.Name == sessionName);
            if (sessionInfo == null)
            {
                var dialog = Instantiate(_dialog, _uiManagerCs.transform);
                dialog.Init("����ID�̃��[���͑��݂��܂���B");
                return;
            }

            ButtonControl(false); //�{�^�����b�N

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //�Z�b�V����������
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //�Z�b�V�����J�n�V�[��
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                SessionName = sessionName, //�Z�b�V������
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
            if (result.Ok == false)
            {
                ButtonControl(true); //�{�^�����b�N����
            }
        }

        /// <summary>
        /// �{�^�����b�N����
        /// </summary>
        /// <param name="state"></param>
        private void ButtonControl(bool state)
        {
            _enterRoomButton.interactable = state;
            _closeButton.interactable = state;
        }
    }
}
