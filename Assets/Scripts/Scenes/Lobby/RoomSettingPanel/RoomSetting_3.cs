using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;
using Common.Network;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_3 : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> _playerNames;

        private void Start()
        {
            this.FixedUpdateAsObservable(). //�v���C���[���X�g�̍X�V
                Subscribe(_ => UpdatePlayerList());
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
