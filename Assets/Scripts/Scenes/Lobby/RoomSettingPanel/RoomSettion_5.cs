using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSettion_5 : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManagerCs;

        /// <summary>
        /// �p�u���b�N�Z�b�V�����ւ̎Q��
        /// </summary>
        public async void JoinPublicSession()
        {
            if (RunnerManager.Instance.SessionInfoList.Count == 0) return; //�Z�b�V���������݂��邩�𔻒�

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //�Z�b�V����������
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
        }

        public void ToRoomSetting_6()
        {
            _uiManagerCs.ChangePanel(5);
        }
    }
}
