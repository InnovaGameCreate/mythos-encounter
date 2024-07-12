using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GotoChangeKeyUI : MonoBehaviour
{
    [SerializeField] private Text[] keyText;

    private void OnEnable()
    {
        keyText[0].text = ConfigKey.dashKey.ToString();
        keyText[1].text = ConfigKey.interactKey.ToString();
        keyText[2].text = ConfigKey.useItemKey.ToString();
        keyText[3].text = ConfigKey.throwAwayItemKey.ToString();
        keyText[4].text = ConfigKey.jumpKey.ToString();
        keyText[5].text = ConfigKey.quietWalkKey.ToString();
        keyText[6].text = ConfigKey.castSpellKey.ToString();
        keyText[7].text = ConfigKey.journalKey.ToString();
        keyText[8].text = ConfigKey.mapKey.ToString();
        keyText[9].text = ConfigKey.turnfaceKey.ToString();
    }
}