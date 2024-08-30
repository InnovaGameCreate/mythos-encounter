using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Prefabs
{
    public class Dialog : MonoBehaviour
    {
        [SerializeField] private TMP_Text _warning; //ダイアログに表示するテキスト

        public delegate void OnCall();
        private OnCall _okCall; //Okボタンが押された時に呼び出されるメソッド
        private OnCall _cancelCall; //Cancelボタンが押された時に呼び出されるメソッド

        /// <summary>
        /// このダイアログの処理を定義する
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
