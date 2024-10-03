using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Common.Network;
using Fusion;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_6 : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _sessionName;

        public async void JoinPrivateSession()
        {
            //�Z�b�V���������݂��邩�𔻒�
            string sessionName = _sessionName.text;
            var sessionInfo = RunnerManager.Instance.SessionInfoList.FirstOrDefault(x => x.Name == sessionName);
            if (sessionInfo == null) return;

            var args = new StartGameArgs()
            {
                GameMode = GameMode.Client, //�Z�b�V����������
                SceneManager = RunnerManager.Runner.GetComponent<NetworkSceneManagerDefault>(), //Fusion�p�̃V�[���J�ڂ��Ǘ�����R���|�[�l���g
                SessionName = sessionName, //�Z�b�V������
                ConnectionToken = Guid.NewGuid().ToByteArray(), //�v���C���[�̐ڑ��g�[�N��
            };

            var result = await RunnerManager.Runner.StartGame(args); //�Z�b�V�����J�n
        }
    }
}
