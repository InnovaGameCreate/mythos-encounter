using Scenes.Ingame.Enemy;
using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RlyehTextEffect : ItemEffect
{
    [SerializeField] private GameObject _fogEffect;
    private bool _used = false;
    public override void OnPickUp()
    {
    }

    public override void OnThrow()
    {

    }

    public override void Effect()
    {
        if(_used) return;
        _used = false;
        var fog = Instantiate(_fogEffect, ownerPlayerStatus.transform.position,Quaternion.identity);
        GameObject[] Enemys = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in Enemys)
        {
            if (enemy != null)
            {
                enemy.GetComponent<EnemyMove>().ResetPosition();
            }
        }
        Destroy(fog, 8f);
        ownerPlayerItem.ConsumeItem(ownerPlayerItem.nowIndex);
    }
}
