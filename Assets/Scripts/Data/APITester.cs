using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
public class APITester : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _textMeshPro;
    [SerializeField]
    private Button _itemTableButton;
    [SerializeField]
    private Button _SpellTableButton;
    [SerializeField]
    private Button _PlayerTableButton;
    [SerializeField]
    private Button _EnemyTableButton;
    void Start()
    {
        _itemTableButton.OnClickAsObservable().Subscribe(_ => ViewItemTable()).AddTo(this);
        _SpellTableButton.OnClickAsObservable().Subscribe(_ => ViewSpellTable()).AddTo(this);
        _PlayerTableButton.OnClickAsObservable().Subscribe(_ => ViewPlayerTable()).AddTo(this);
        _EnemyTableButton.OnClickAsObservable().Subscribe(_ => ViewEnemyTable()).AddTo(this);
    }

    private void ViewItemTable()
    {
        var data = WebDataRequest.GetItemDataArrayList;
        Debug.Log($"ViewItemTable : {data.Count}");
        _textMeshPro.text = "id,name,explanation,catgory,price\n";
        foreach (var item in data)
        {
            _textMeshPro.text += $"{item.ItemId},{item.Name},{item.Description},{item.ItemCategory},{item.Price}\n";
        }
    }
    private void ViewSpellTable()
    {
        var data = WebDataRequest.GetSpellDataArrayList;
        Debug.Log($"ViewSpellTable : {data.Count}");
        _textMeshPro.text = "id,name,explanation,unlockExplanation\n";
        foreach (var item in data)
        {
            _textMeshPro.text += $"{item.SpellId},{item.Name},{item.Explanation},{item.unlockExplanation}\n";
        }
    }
    private void ViewPlayerTable()
    {
        var data = WebDataRequest.GetPlayerDataArrayList;
        Debug.Log($"ViewPlayerTable : {data.Count}");
        _textMeshPro.text = "id,name,createdDate,endDate,money,item,enemy,spell\n";
        foreach (var item in data)
        {
            _textMeshPro.text += $"{item.Id},{item.Name},{item.Created},{item.Ended},{item.Money},{item.Items.Length},{item.MythCreature.Length},{item.Spell.Length}\n";
        }
    }
    private void ViewEnemyTable()
    {
        var data = WebDataRequest.GetEnemyDataArrayList;
        Debug.Log($"ViewPlayerTable : {data.Count}");
        _textMeshPro.text = "id,name,hp,stamia,armor,walkSpeed,dashSpeed,attack,actionCoolTime,sapell,san\n";
        foreach (var item in data)
        {
            _textMeshPro.text += $"{item.EnemyId},{item.Name},{item.Stamina},{item.Armor},{item.WalkSpeed},{item.DashSpeed},{item.AttackPower},{item.ActionCooltime},{item.Spell.Length},{item.San}\n";
        }
    }
}
