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

    private int _changekeynum;  //�L�[�̕ۑ���̔��ʂɎg�p
    private static int _keynum = 10;//�g���L�[�̐�
    private KeyCode _code;  //�����ꂽ�L�[
    private KeyCode[] _inputkeys = new KeyCode[_keynum];

    [SerializeField] private Text putKeyShowText;   //�������L�[��\������
    [SerializeField] private Text saveResultText;   //�ۑ��������Ƃ������͕ۑ��Ɏ��s�������Ƃ�\������

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
        putKeyShowText.text = "���͂����L�[���ۑ�����܂�";
        saveResultText.text = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(KeyCode.Mouse0)
            && Input.anyKeyDown)//�L�[�������ꂽ��
        {
            InputKey();
        }
    }

    //Mouse0�������L�[�̓��͎�t���s�������B_key��ConfigKey�̃L�[
    public void InputKey()
    {
        if (saveResultText.text != null)//�ۑ����܂������̕\�����폜
        {
            saveResultText.text = null;
        }
        foreach (KeyCode _putcode in Enum.GetValues(typeof(KeyCode)))//�����ꂽ�L�[�������T�m
        {
            if (Input.GetKeyDown(_putcode))
            {
                _code = _putcode;
                putKeyShowText.text = _code.ToString();
            }
        }
    }

    //Mouse0�̓��͎�t�B�ۑ��{�^���������ۂɓo�^�������L�[�Ƃ���
    //�������Ă��܂����ߑ��̓��͎�t�ƕ����Ă���B
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
            saveResultText.text = "�ۑ��������L�[����͂��Ă�������";
            return;
        }
        for (int i = 0; i < _keynum; i++)//�L�[�����Ɏg���ĂȂ����m�F
        {
            if (_inputkeys[i] == _code)
            {
                saveResultText.text = "�����g���Ă���L�[�ł�";//�_�C�A���O��\���B�ۑ����Ȃ�
                return;
            }
            else if (i == _keynum - 1)//�g���Ă��Ȃ����
            {
                switch (_changekeynum)
                {
                    case (int)Keynum.dashKeynum:
                        _inputkeys[_changekeynum] = _code;
                        ConfigKey.dashKey = _code;              //�ۑ�����
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
                    default:Debug.Log("�z�肳��Ă��Ȃ��l�����͂���Ă��܂�"); break;
                }
                saveResultText.text = "�ۑ����܂���";
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
