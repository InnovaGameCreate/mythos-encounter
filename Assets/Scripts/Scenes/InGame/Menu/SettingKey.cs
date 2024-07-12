using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SettingKey : MonoBehaviour
{
    public enum Keynum
    {
        dashKeynum,
        interactKeynum,
        useItemKeynum,
        throwAwayItemKeynum,
        jumpKeynum,
        quietWalkKeynum,
        castSpellKeynum,
        journalKeynum,
        mapKeynum,
        turnfaceKeynum
    }

    [SerializeField] private GameObject gotoChangeKeyUI;

    private int _changekeynum;  //キーの保存先の判別に使用
    private static int _keynum = 10;//使うキーの数
    private KeyCode _code;  //押されたキー
    private KeyCode[] _inputkeys = new KeyCode[_keynum];

    [SerializeField] private Text putKeyShowText;   //押したキーを表示する
    [SerializeField] private Text saveResultText;   //保存したこともしくは保存に失敗したことを表示する

    // Start is called before the first frame update
    void Start()
    {
        _inputkeys[(int)Keynum.dashKeynum] = ConfigKey.dashKey;
        _inputkeys[(int)Keynum.interactKeynum] = ConfigKey.interactKey;
        _inputkeys[(int)Keynum.useItemKeynum] = ConfigKey.useItemKey;
        _inputkeys[(int)Keynum.throwAwayItemKeynum] = ConfigKey.throwAwayItemKey;
        _inputkeys[(int)Keynum.journalKeynum] = ConfigKey.journalKey;
        _inputkeys[(int)Keynum.quietWalkKeynum] = ConfigKey.quietWalkKey;
        _inputkeys[(int)Keynum.castSpellKeynum] = ConfigKey.castSpellKey;
        _inputkeys[(int)Keynum.journalKeynum] = ConfigKey.journalKey;
        _inputkeys[(int)Keynum.mapKeynum] = ConfigKey.mapKey;
        _inputkeys[(int)Keynum.turnfaceKeynum] = ConfigKey.turnfaceKey;
    }

    private void OnEnable()
    {
        _code = KeyCode.None;
        putKeyShowText.text = "入力したキーが保存されます";
        saveResultText.text = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(KeyCode.Mouse0)
            && Input.anyKeyDown)//キーが押されたら
        {
            InputKey();
        }
    }

    //Mouse0を除くキーの入力受付を行う処理。_keyはConfigKeyのキー
    public void InputKey()
    {
        if (saveResultText.text != null)//保存しました等の表示を削除
        {
            saveResultText.text = null;
        }
        foreach (KeyCode _putcode in Enum.GetValues(typeof(KeyCode)))//押されたキーが何か探知
        {
            if (Input.GetKeyDown(_putcode))
            {
                _code = _putcode;
                putKeyShowText.text = _code.ToString();
            }
        }
    }

    //Mouse0の入力受付。保存ボタンを押す際に登録したいキーとして
    //反応してしまうため他の入力受付と分けてある。
    public void InputMouse0()
    {
        if (saveResultText.text != null)
        {
            saveResultText.text = null;
        }
        _code = KeyCode.Mouse0;
        putKeyShowText.text = _code.ToString();
    }

    public void SaveKey()
    {
        if (_code == KeyCode.None)
        {
            saveResultText.text = "保存したいキーを入力してください";
            return;
        }
        for (int i = 0; i < _keynum; i++)//キーが既に使われてないか確認
        {
            if (_inputkeys[i] == _code)
            {
                saveResultText.text = "もう使われているキーです";//ダイアログを表示。保存しない
                return;
            }
            else if (i == _keynum - 1)//使われていなければ
            {
                switch (_changekeynum)
                {
                    case (int)Keynum.dashKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.dashKey = _code;              //保存する
                        break;
                    case (int)Keynum.interactKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.interactKey = _code;
                        break;
                    case (int)Keynum.useItemKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.useItemKey = _code;
                        break;
                    case (int)Keynum.throwAwayItemKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.throwAwayItemKey = _code;
                        break;
                    case (int)Keynum.jumpKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.jumpKey = _code;
                        break;
                    case (int)Keynum.quietWalkKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.quietWalkKey = _code;
                        break;
                    case (int)Keynum.castSpellKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.castSpellKey = _code;
                        break;
                    case (int)Keynum.journalKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.journalKey = _code;
                        break;
                    case (int)Keynum.mapKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.mapKey = _code;
                        break;
                    case (int)Keynum.turnfaceKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.turnfaceKey = _code;
                        break;
                    default:Debug.Log("想定されていない値が入力されています"); break;
                }
                saveResultText.text = "保存しました";
            }
        }
    }

    public void ChangeKey(int _ccode)
    {
        _changekeynum = _ccode;
    }

    public void CancelChangeKey()
    {
        gotoChangeKeyUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
