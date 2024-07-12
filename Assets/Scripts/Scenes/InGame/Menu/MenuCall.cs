using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCall : MonoBehaviour
{
    [SerializeField] private GameObject MenuUI;
    [SerializeField] private GameObject OptionUI;
    [SerializeField] private GameObject VolumeUI;
    [SerializeField] private Toggle OptionToggle;
    [SerializeField] private Toggle VolumeToggle;
    private bool _menuActive = false;
    private bool _optionActive = true;
    private bool _volumeActive = false;
    void Start()
    {
        MenuUI.SetActive(_menuActive);
    }

    // Update is called once per frame
    void Update()
    {
        //メニューの呼び出しを行う.ゲーム内容の欄が始めに出るようにしている
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _menuActive = !_menuActive;
            OptionUI.SetActive(_optionActive);
            VolumeUI.SetActive(_volumeActive);
            OptionToggle.isOn = _optionActive;
            VolumeToggle.isOn = _volumeActive;
            MenuUI.SetActive(_menuActive);
        }
    }
}
