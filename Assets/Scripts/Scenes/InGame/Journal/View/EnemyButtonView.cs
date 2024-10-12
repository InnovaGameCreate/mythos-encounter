using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EnemyButtonView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Button _button;
    public Button button { get { return _button; } }
    public void NameSet(string name)
    {
        _name.text = name;
    }
}
