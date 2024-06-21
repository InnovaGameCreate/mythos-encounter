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
        [SerializeField, Tooltip("���U���g�p�̃L�����o�X")]
        private GameObject _resultCanvas;
        [SerializeField, Tooltip("�E�o�����̃e�L�X�g")]
        private TextMeshProUGUI _successEscape;
        [SerializeField, Tooltip("�E�o���Ԃ̃e�L�X�g")]
        private TextMeshProUGUI _escapeTime;
        [SerializeField, Tooltip("��Փx�̃e�L�X�g")]
        private TextMeshProUGUI _diffculty;
        [SerializeField, Tooltip("���j�[�N�A�C�e���擾�̃e�L�X�g")]
        private TextMeshProUGUI _getUniqueItem;
        [SerializeField, Tooltip("�������̃e�L�X�g")]
        private TextMeshProUGUI _firstContact;
        [SerializeField, Tooltip("���v���z�̃e�L�X�g")]
        private TextMeshProUGUI _totalMoney;
        [SerializeField, Tooltip("Lobby�ɖ߂�p�̃{�^��")]
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
                 _escapeTime.text = $"�E�o���ԁ@�@�@�@�@�@�@�@�@{min}:{sec}";
                 _diffculty.text = $"��Փx�@�@�@�@�@�@�@�@ �@�@{resultValue.level}";
                 _getUniqueItem.text = $"���j�[�N�A�C�e���̎擾�@{(resultValue.getUnique ? "�擾" : "���擾")}";
                 _firstContact.text = $"�������@�@�@�@�@�@�@�@�@{(resultValue.firstContact ? "������" : "�����ς�")}";
                 _totalMoney.text = $"���v���z�@�@�@�@�@�@�@�@�@{resultValue.totalMoney}$";
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