using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    public class HealOrbEffect : ItemEffect
    {
        public override void OnPickUp()
        {
            
        }

        public override void Effect()
        {
            ownerPlayerStatus.ChangeHealth(100, "Heal");
            ownerPlayerItem.ThrowItem(ownerPlayerItem.nowIndex);
        }

    }
}

