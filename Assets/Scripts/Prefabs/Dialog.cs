using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Prefabs
{
    public class Dialog : MonoBehaviour
    {
        [SerializeField] private TMP_Text _warning; //�_�C�A���O�ɕ\������e�L�X�g

        public delegate void OnCall();
        private OnCall _okCall; //Ok�{�^���������ꂽ���ɌĂяo����郁�\�b�h
        private OnCall _cancelCall; //Cancel�{�^���������ꂽ���ɌĂяo����郁�\�b�h

        /// <summary>
        /// ���̃_�C�A���O�̏������`����
        /// </summary>
        /// <param name="text"></param>
        public void Init(string text)
        {
            _warning.text = text;
        }

        public void Init(string text, OnCall okCall, OnCall cancelCall)
        {
            _warning.text = text;
            _okCall = okCall;
            _cancelCall = cancelCall;
        }

        public void OnOkButton()
        {
            if (_okCall != null) _okCall();
            Destroy(this.gameObject);
        }

        public void OnCancelButton()
        {
            if (_cancelCall != null) _cancelCall();
            Destroy(this.gameObject);
        }
    }
}
