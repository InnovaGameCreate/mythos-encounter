using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using Data;
using Unity.VisualScripting;
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
    [SerializeField]
    private Button _itemFacadeButton;
    [SerializeField]
    private Button _SpellFacadeButton;
    [SerializeField]
    private Button _EnemyFacadeButton;
    void Start()
    {
        _itemTableButton.OnClickAsObservable().Subscribe(_ => ViewItemTable()).AddTo(this);
        _SpellTableButton.OnClickAsObservable().Subscribe(_ => ViewSpellTable()).AddTo(this);
        _PlayerTableButton.OnClickAsObservable().Subscribe(_ => ViewPlayerTable()).AddTo(this);
        _EnemyTableButton.OnClickAsObservable().Subscribe(_ => ViewEnemyTable()).AddTo(this);

        _itemFacadeButton.OnClickAsObservable().Subscribe(_ => ViewItemFacade()).AddTo(this);
        _SpellFacadeButton.OnClickAsObservable().Subscribe(_ => ViewSpellFacade()).AddTo(this);
        _EnemyFacadeButton.OnClickAsObservable().Subscribe(_ => ViewEnemyFacade()).AddTo(this);
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
    private void ViewItemFacade()
    {
        _textMeshPro.text = "Owned\nid,name,explanation,catgory,price\n";
        var data = PlayerInformationFacade.Instance.GetItem(PlayerInformationFacade.ItemRequestType.Owned);
        foreach (var item in data)
        {
            _textMeshPro.text += $"id {item.Key} のアイテムを {item.Value} 個\n";
        }
        _textMeshPro.text += "not Owned\nid,name,explanation,catgory,price\n";
        data = PlayerInformationFacade.Instance.GetItem(PlayerInformationFacade.ItemRequestType.NotOwned);
        foreach (var item in data)
        {
            _textMeshPro.text += $"id {item.Key} のアイテムを {item.Value} 個\n";
        }
    }
    private void ViewSpellFacade()
    {
        _textMeshPro.text = "Owned\nid,name,explanation,unlockExplanation\n";
        var data = PlayerInformationFacade.Instance.GetSpell(PlayerInformationFacade.spellRequestType.Owned);
        foreach (var item in data.Values)
        {
            _textMeshPro.text += $"{item.SpellId},{item.Name},{item.Explanation},{item.unlockExplanation}\n";
        }
        _textMeshPro.text += "not Owned\nid,name,explanation,unlockExplanation\n";
        data = PlayerInformationFacade.Instance.GetSpell(PlayerInformationFacade.spellRequestType.NotOwned);
        foreach (var item in data.Values)
        {
            _textMeshPro.text += $"{item.SpellId},{item.Name},{item.Explanation},{item.unlockExplanation}\n";
        }
    }
    private void ViewEnemyFacade()
    {
        _textMeshPro.text = "Owned\nid,name,explanation,catgory,price\n";
        var data = PlayerInformationFacade.Instance.GetEnemy(PlayerInformationFacade.EnemyRequestType.Met);
        foreach (var item in data)
        {
            _textMeshPro.text += $"id {item.Key} の敵と {item.Value} 回遭遇\n";
        }
        _textMeshPro.text += "not Owned\nid,name,explanation,catgory,price\n";
        data = PlayerInformationFacade.Instance.GetEnemy(PlayerInformationFacade.EnemyRequestType.NotMet);
        foreach (var item in data.Values)
        {
            _textMeshPro.text += $"未遭遇の敵のidは {item}\n";
        }
    }
}
