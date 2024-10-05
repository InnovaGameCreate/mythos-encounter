using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Common.Network;
using Common.UI;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_3 : MonoBehaviour
    {
        [SerializeField] private Button _publicButton;
        [SerializeField] private Button _privateButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private Dialog _dialog;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        /// <summary>
        /// �p�u���b�N�Z�b�V�����ւ̎Q��
        /// </summary>
        public async void JoinPublicSession()
        {
            //�Z�b�V���������݂��邩�𔻒�
            if (RunnerManager.Instance.SessionInfoList.Count == 0)
            {
                var dialog = Instantiate(_dialog, _uiManagerCs.transform);
                dialog.Init("���[�������݂��܂���");
                return;
            }

            ButtonControl(false); //�{�^�����b�N

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //�Z�b�V����������
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //�Z�b�V�����J�n�V�[��
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
            if (result.Ok == false)
            {
                ButtonControl(true); //�{�^�����b�N����
            }
        }

        public void ToRoomSetting_4()
        {
            _uiManagerCs.ChangePanel(3); //�v���C�x�[�g�Q�����
        }

        /// <summary>
        /// �{�^�����b�N����
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
