using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

namespace Network
{
    public class Develop_ToMatchingSceneManager : MonoBehaviour
    {
        [SerializeField] private NetworkRunner _runnerPrefab; //NetworkRunner��Prefab

        public async void ToSessionLobbyButton()
        {
            //�S�Ă�Button��G��Ȃ���Ԃɂ���
            var buttons = FindObjectsOfType<Button>();
            foreach (var button in buttons) button.interactable = false;

            //NetworkRunner��z�u����
            var runner = Instantiate(_runnerPrefab);
            runner.ProvideInput = true; //���͌���(InputAuthority)���g��
            DontDestroyOnLoad(runner); //DontDestroy��t���Ȃ��ƃV�[���J�ڎ��ɏ��ł���(HostMigration�L����)

            //���r�[�ւ̎Q��
            var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);
            if (result.Ok)
            {
                Debug.Log("Join SessionLobby");
                SceneManager.LoadScene("SessionLobby"); //SessionLobby�V�[���ւ̑J��
            }
            else
            {
                Debug.LogError($"Error : {result.ShutdownReason}"); //�ڑ��̎��s
                foreach (var button in buttons) button.interactable = true; //Interactable������
            }
        }
    }
}
