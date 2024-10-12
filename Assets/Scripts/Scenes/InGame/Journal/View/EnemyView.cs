using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;

public class EnemyView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _rightPage;
    [SerializeField] private EnemyButtonView _enemyButton;
    [SerializeField] private Transform _content;
    public override void Init()
    {
        WebDataRequest.instance.OnEndLoad.Subscribe(_ =>
        {
            Debug.Log("CompatibilityView.Init");
            var enemyList = WebDataRequest.GetEnemyDataArrayList;
            foreach (var enemy in enemyList)
            {
                var enemyButton = Instantiate(_enemyButton, _content);
                enemyButton.NameSet(enemy.Name);
                enemyButton.button.OnClickAsObservable().Subscribe(_ => _rightPage.text = text(MasterData.instance.master.EnemyDetail[enemy.EnemyId- 1])).AddTo(this);
            }
        }).AddTo(this);
    }

    public string text(EnemyDetail detail)
    {
        return $"<size=22>{detail.Name}</size>\n\n {detail.BackGround}\n\n {detail.Feature}\n\n {detail.Counter}\n\n  {detail.Trace}";
    }
}
