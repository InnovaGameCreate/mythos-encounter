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
        public void OnToCreateSessionButton()
        {
            SceneManager.LoadScene("CreateSession");
        }

        public void OnBackButton()
        {
            //�S�Ẵ{�^����G��Ȃ���Ԃɂ���
            var buttons = FindObjectsOfType<Button>();
            foreach (var button in buttons) button.interactable = false;

            //�I�����C����Ԃ��痣�E����
            RunnerManager.Runner.Shutdown();
            RunnerManager.Runner = null;
            RunnerManager.Instance = null;

            SceneManager.LoadScene("Develop_ToMatchingScene");
        }
    }
}
