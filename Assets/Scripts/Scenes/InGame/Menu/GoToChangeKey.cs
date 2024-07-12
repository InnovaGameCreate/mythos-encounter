using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToChangeKey : MonoBehaviour
{
    [SerializeField] private GameObject changeKeyUI;
    private SettingKey _setkeyCs;

    void Start()
    {
        _setkeyCs = changeKeyUI.GetComponent<SettingKey>();
        changeKeyUI.SetActive(false);
    }

    private void GoChangeKeyUI(int _keynum)
    {
        changeKeyUI.SetActive(true);//キー変更用のオブジェクトをアクティブに
        _setkeyCs.ChangeKey(_keynum);//
        this.gameObject.SetActive(false);
    }

    public void ChangeDashKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.dashKeynum);
    }
    public void ChangeIntaractKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.interactKeynum);
    }
    public void ChangeUseItemKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.useItemKeynum);
    }
    public void ChangeThrowAwayItemKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.throwAwayItemKeynum);
    }
    public void ChangeJumpKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.jumpKeynum);
    }
    public void ChangeQuietWalkKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.quietWalkKeynum);
    }
    public void ChangeCastSpellKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.castSpellKeynum);
    }
    public void ChangeJournalKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.journalKeynum);
    }
    public void ChangeMapKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.mapKeynum);
    }
    public void ChangeTurnFaceKey()
    {
        GoChangeKeyUI((int)SettingKey.Keynum.turnfaceKeynum);
    }
}
