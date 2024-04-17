using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOrbEffect : ItemEffect
{
    PlayerStatus _myPlayerStatus;
    private void Start()
    {
        base.SetUp();
        _myPlayerStatus = GameObject.FindWithTag("Player").GetComponent<PlayerStatus>();
    }

    public override void Effect()
    {
        _myPlayerStatus.ChangeHealth(100, "Heal");
    }

}
