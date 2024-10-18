using TMPro;
using UnityEngine;
using UniRx;
using System.Text.RegularExpressions;

namespace Scenes.Ingame.Journal
{
    public class ItemView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _rightPage;
        [SerializeField] private Transform _content;
        [SerializeField] private NameButtonView _itemButton;

        public override void Init()
        {
            WebDataRequest.instance.OnEndLoad.Subscribe(_ =>
            {
                var itemList = WebDataRequest.GetItemDataArrayList;
                foreach (var item in itemList)
                {
                    var itemButton = Instantiate(_itemButton, _content);
                    itemButton.NameSet(item.Name);
                    itemButton.button.OnClickAsObservable().Subscribe(_ => _rightPage.text = text(item)).AddTo(this);
                }
            }).AddTo(this);
        }

        public string text(ItemDataStruct detail)
        {
            return $"<size=20>{detail.Name}</size>\n\n<size=18>ê‡ñæ</size>\n{Regex.Unescape(detail.Description)}";
        }
    }
}