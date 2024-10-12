using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Common.Network;

namespace Scenes.MultiLobby.StartDoor
{
    public class InGameStart : MonoBehaviour
    {
        [SerializeField] private int _inGameIndex = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartGame();
            }
        }

        /// <summary>
        /// ÉCÉìÉQÅ[ÉÄÇ÷à⁄ÇÈ
        /// </summary>
        private void StartGame()
        {
            if (RunnerManager.Runner.IsServer == false) return;
            RunnerManager.Runner.LoadScene(SceneRef.FromIndex(_inGameIndex));
        }
    }
}
