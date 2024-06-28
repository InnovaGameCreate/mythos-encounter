using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Manager;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace Scenes.Ingame.InGameSystem.UI
{
    public class ResultPresenter : MonoBehaviour
    {
        private Sequence _sequence;
        [SerializeField, Tooltip("リザルト用のキャンバス")]
        private GameObject _resultCanvas;
        [SerializeField, Tooltip("脱出成功のテキスト")]
        private TextMeshProUGUI _successEscape;
        [SerializeField, Tooltip("脱出時間のテキスト")]
        private TextMeshProUGUI _escapeTime;
        [SerializeField, Tooltip("難易度のテキスト")]
        private TextMeshProUGUI _diffculty;
        [SerializeField, Tooltip("ユニークアイテム取得のテキスト")]
        private TextMeshProUGUI _getUniqueItem;
        [SerializeField, Tooltip("初遭遇のテキスト")]
        private TextMeshProUGUI _firstContact;
        [SerializeField, Tooltip("合計金額のテキスト")]
        private TextMeshProUGUI _totalMoney;
        [SerializeField, Tooltip("Lobbyに戻る用のボタン")]
        private Button _lobbyBackButton;
        void Start()
        {
            _resultCanvas.SetActive(false);
            IngameManager.Instance.OnResult
                .Subscribe(_ =>
                {
                    _resultCanvas.SetActive(true);
                }).AddTo(this);

            ResultManager.Instance.OnResultValue
                .Subscribe(value =>
                {
                    Display(value);
                }).AddTo(this);
        }
        private void Display(ResultValue resultValue)
        {
            _sequence = DOTween.Sequence();
            _sequence
             .AppendCallback(() =>
             {
                 _lobbyBackButton.interactable = false;
                 _successEscape.color = new Color(0, 0, 0, 0);
                 _escapeTime.color = new Color(0, 0, 0, 0);
                 _diffculty.color = new Color(0, 0, 0, 0);
                 _getUniqueItem.color = new Color(0, 0, 0, 0);
                 _firstContact.color = new Color(0, 0, 0, 0);
                 _totalMoney.color = new Color(0, 0, 0, 0);
                 string min = (resultValue.time / 60).ToString("D2");
                 string sec = (resultValue.time % 60).ToString("D2");
                 _escapeTime.text = $"脱出時間　　　　　　　　　{min}:{sec}";
                 _diffculty.text = $"難易度　　　　　　　　 　　{resultValue.level}";
                 _getUniqueItem.text = $"ユニークアイテムの取得　{(resultValue.getUnique ? "取得" : "未取得")}";
                 _firstContact.text = $"初遭遇　　　　　　　　　{(resultValue.firstContact ? "初遭遇" : "遭遇済み")}";
                 _totalMoney.text = $"合計金額　　　　　　　　　{resultValue.totalMoney}$";
             })
            .Append(_successEscape.DOFade(1, 0.5f))
            .AppendInterval(0.2f)
            .Append(_escapeTime.DOFade(1, 0.5f))
            .AppendInterval(0.2f)
            .Append(_diffculty.DOFade(1, 0.5f))
            .AppendInterval(0.2f)
            .Append(_getUniqueItem.DOFade(1, 0.5f))
            .AppendInterval(0.2f)
            .Append(_firstContact.DOFade(1, 0.5f))
            .AppendInterval(0.2f)
            .Append(_totalMoney.DOFade(1, 0.5f))
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                _lobbyBackButton.interactable = true;
            });
        }
    }
}