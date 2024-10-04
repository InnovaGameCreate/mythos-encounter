using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_3 : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManagerCs;
        [SerializeField] private int _sessionStartSceneIndex = 0;

        /// <summary>
        /// �p�u���b�N�Z�b�V�����ւ̎Q��
        /// </summary>
        public async void JoinPublicSession()
        {
            if (RunnerManager.Instance.SessionInfoList.Count == 0) return; //�Z�b�V���������݂��邩�𔻒�

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //�Z�b�V����������
                Scene = SceneRef.FromIndex(_sessionStartSceneIndex), //�Z�b�V�����J�n�V�[��
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
        }

        public void ToRoomSetting_4()
        {
            _uiManagerCs.ChangePanel(3);
        }
    }
}
