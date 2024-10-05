using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Common.UI
{
    public class Dialog : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageTMP;

        //èâä˙âª
        public void Init(string message)
        {
            _messageTMP.text = message;
        }

        //Close
        public void OnCloseButton()
        {
            Destroy(this.gameObject);
        }
    }
}
