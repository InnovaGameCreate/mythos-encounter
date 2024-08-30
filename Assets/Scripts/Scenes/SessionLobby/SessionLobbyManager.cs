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
            //全てのボタンを触れない状態にする
            AllInteractable(false);

            //オンライン状態から離脱する
            RunnerManager.Runner.Shutdown();
            RunnerManager.Runner = null;
            RunnerManager.Instance = null;

            SceneManager.LoadScene("Develop_ToMatchingScene");
        }

        /// <summary>
        /// 全てのUIオブジェクトのInteractableを操作する
        /// </summary>
        /// <param name="state"></param>
        public void AllInteractable(bool state)
        {
            _backButton.interactable = state;
            _toCreateSessionButton.interactable = state;
        }
    }
}
