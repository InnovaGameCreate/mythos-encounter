using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using Network;

namespace Scenes.SessionLobby
{
    public class SessionLobbyManager : MonoBehaviour
    {
        [Header("Canvas Objects")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _toCreateSessionButton;

        public void OnToCreateSessionButton()
        {
            SceneManager.LoadScene("CreateSession");
        }

        public void OnBackButton()
        {
            //�S�Ẵ{�^����G��Ȃ���Ԃɂ���
            AllInteractable(false);

            //�I�����C����Ԃ��痣�E����
            RunnerManager.Runner.Shutdown();
            RunnerManager.Runner = null;
            RunnerManager.Instance = null;

            SceneManager.LoadScene("Develop_ToMatchingScene");
        }

        /// <summary>
        /// �S�Ă�UI�I�u�W�F�N�g��Interactable�𑀍삷��
        /// </summary>
        /// <param name="state"></param>
        public void AllInteractable(bool state)
        {
            _backButton.interactable = state;
            _toCreateSessionButton.interactable = state;
        }
    }
}
