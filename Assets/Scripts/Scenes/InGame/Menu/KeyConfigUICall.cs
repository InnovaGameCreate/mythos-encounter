using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyConfigUICall : MonoBehaviour
{
    [SerializeField] private GameObject gotoChangeKeyUI;
    [SerializeField] private GameObject changeKeyUI;
    private void OnEnable()
    {
        changeKeyUI.SetActive(false);
        gotoChangeKeyUI.SetActive(true);
    }
}
