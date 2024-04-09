using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

namespace Scenes.Ingame.Player
{ 
    /// <summary>
    /// PlayerStatusの情報をもとにゲーム内UIにプレイヤー全員の情報を表示させる為のスクリプト
    /// 後ほどネットワークでの、複数人の情報の表示に対応できる形を心がける
    /// </summary>
    public class DisplayPlayerStatusManager : MonoBehaviour
    {
        [SerializeField] private Slider[] _healthSliders;
        [SerializeField] private Slider[] _sanValueSliders;
        [SerializeField] private TMP_Text[] _healthText;
        [SerializeField] private TMP_Text[] _sanText;

        /// <summary>
        /// Sliderの値を変える為の関数
        /// </summary>
        /// <param name="value">Slinder.Valueに代入する値</param>
        /// <param name="ID">プレイヤーID</param>
        /// <param name="mode">Health(体力), SanValue(SAN値)どちらを変更するのかを決定</param>
        public void ChangeSliderValue(int value , int ID, string mode)
        {
            if (mode == "Health")
            {
                _healthSliders[ID].value = value;
                _healthText[ID].text = value.ToString();
            }

            else if (mode == "SanValue")
            { 
                _sanValueSliders[ID].value = value;
                _sanText[ID].text = value.ToString();
            }
                
        }
    }
}

