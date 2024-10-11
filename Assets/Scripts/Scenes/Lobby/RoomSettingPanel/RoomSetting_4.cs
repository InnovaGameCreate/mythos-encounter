using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_4 : MonoBehaviour
    {
        [SerializeField] private TMP_Text _sessionName;
        [SerializeField] private List<TMP_Text> _playerNames;

        private void Start()
        {
            this.FixedUpdateAsObservable(). //�Z�b�V�������̍X�V
                Subscribe(_ => UpdateSessionName());

            this.FixedUpdateAsObservable(). //�v���C���[���X�g�̍X�V
                Subscribe(_ => UpdatePlayerList());
        }

        /// <summary>
        /// �Z�b�V�������̍X�V
        /// </summary>
        private void UpdateSessionName()
        {
            string sessionName = RunnerManager.Runner.SessionInfo.Name;
            _sessionName.text = "���[��ID�F" + sessionName;
        }

        /// <summary>
        /// �v���C���[���X�g�̍X�V
        /// </summary>
        private void UpdatePlayerList()
        {
            var playerInfos = FindObjectsOfType<PlayerInfo>();

            for (int i = 0; i < _playerNames.Count; i++)
            {
                if (i < playerInfos.Length)
                {
                    _playerNames[i].text = playerInfos[i].userName;
                }
                else
                {
                    _playerNames[i].text = "�����o�[�����܂���";
                }
            }
        }
    }
}
