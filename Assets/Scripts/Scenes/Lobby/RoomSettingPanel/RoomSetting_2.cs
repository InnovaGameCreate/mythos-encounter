using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_2 : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private int _playerNum = 4;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        public void ToRoomSetting_3()
        {
            _uiManagerCs.ChangePanel(2);
            SessionCreate(); //�p�u���b�N�Z�b�V�����쐬
        }

        public void ToRoomSetting_4()
        {
            _uiManagerCs.ChangePanel(3);
            SessionCreate(Guid.NewGuid().ToString()); //�v���C�x�[�g�Z�b�V�����쐬
        }

        /// <summary>
        /// �Z�b�V�����쐬
        /// </summary>
        private async void SessionCreate()
        {
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //�Z�b�V����������
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(),
                PlayerCount = _playerNum, //�ő�l���̌���
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
        }

        /// <summary>
        /// �Z�b�V�����쐬�i�v���C�x�[�g�j
        /// </summary>
        /// <param name="sessionName"></param>
        private async void SessionCreate(string sessionName)
        {
            Debug.Log("Session Name : " + sessionName);

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Host, //�Z�b�V����������
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                SessionName = sessionName, //�Z�b�V������
                PlayerCount = _playerNum, //�ő�l���̌���
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
        }
    }
}
