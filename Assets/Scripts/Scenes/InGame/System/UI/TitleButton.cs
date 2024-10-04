using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Scenes.Ingame.InGameSystem.UI
{
    public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Button _gameStartButton;
        [SerializeField] private Button _optionButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _settingApplyButton;
        [SerializeField] private GameObject _settingWindow;
        [SerializeField] private GameObject _type1Button;
        [SerializeField] private GameObject _type2Button;
        [SerializeField] private GameObject _typeWindow;
        [SerializeField] private Button _nameApplyButton;
        [SerializeField] private GameObject _nameWindow;
 
        // マウスがどのボタンに入ったかの判定
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!TitleState.GetSetWindow)
            {
                if (this.gameObject == _gameStartButton.gameObject)
                {
                    _buttonText.text = "ゲームを開始します。";
                }
                else if (this.gameObject == _optionButton.gameObject)
                {
                    _buttonText.text = "設定を変更・確認します。";
                }
                else if (this.gameObject == _quitButton.gameObject)
                {
                    _buttonText.text = "ゲームを終了しウィンドウを閉じます。";
                }
                else if (this.gameObject == _continueButton.gameObject)
                {
                    _buttonText.text = "前回のキャラクターで再開します。";
                }
                else if (this.gameObject == _newGameButton.gameObject)
                {
                    _buttonText.text = "キャラクターを新規作成します。";
                }
            }
        }

        // マウスがオブジェクトから離れたらテキストを空にする
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!TitleState.GetSetWindow)
            {
                _buttonText.text = "";
            }
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            if (!TitleState.GetSetWindow)
            {
                if (this.gameObject == _gameStartButton.gameObject)
                {
                    _gameStartButton.gameObject.SetActive(false);
                    _optionButton.gameObject.SetActive(false);
                    _quitButton.gameObject.SetActive(false);
                    _continueButton.gameObject.SetActive(true);
                    _newGameButton.gameObject.SetActive(true);
                }
                else if (this.gameObject == _optionButton.gameObject)
                {
                    TitleState.GetSetWindow = true;
                    _settingWindow.SetActive(true);
                }
                else if (this.gameObject == _quitButton.gameObject)
                {

                }
                else if (this.gameObject == _continueButton.gameObject)
                {

                }
                else if (this.gameObject == _newGameButton.gameObject)
                {
                    TitleState.GetSetWindow = true;
                    _typeWindow.SetActive(true);
                }
            }

            if (this.gameObject == _settingButton.gameObject)
            {
                _settingWindow.SetActive(false);
                TitleState.GetSetWindow = false;
                _buttonText.text = "";
            }
            else if (this.gameObject == _settingApplyButton.gameObject)
            {
                TitleState.GetSetWindow = false;
                _settingWindow.SetActive(false);
                _buttonText.text = "";
            }
            else if (this.gameObject == _type1Button)
            {
                _nameWindow.SetActive(true);
                _typeWindow.SetActive(false);
            }
            else if (this.gameObject == _type2Button)
            {
                _nameWindow.SetActive(true);
                _typeWindow.SetActive(false);
            }
            else if (this.gameObject == _nameApplyButton.gameObject)
            {
                TitleState.GetSetWindow = false;
                _nameWindow.SetActive(false);
            }
        }
    }
}