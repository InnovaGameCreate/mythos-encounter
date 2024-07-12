using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUICall : MonoBehaviour
{
    [SerializeField] private GameObject GameContentUI;
    [SerializeField] private GameObject KeyConfigUI;
    [SerializeField] private GameObject ReturnLobbyUI;
    [SerializeField] private Toggle GameContentToggle;
    [SerializeField] private Toggle KeyConfigToggle;
    [SerializeField] private Toggle ReturnLobbyToggle;
    private bool _gameContentActive = true;
    private bool _keyConfigActive = false;
    private bool _returnLobbyActive = false;

    private void OnEnable()
    {
        GameContentUI.SetActive(_gameContentActive);
        KeyConfigUI.SetActive(_keyConfigActive);
        ReturnLobbyUI.SetActive(_returnLobbyActive);
        GameContentToggle.isOn = _gameContentActive;
        KeyConfigToggle.isOn = _keyConfigActive;
        ReturnLobbyToggle.isOn = _returnLobbyActive;

    }
}
