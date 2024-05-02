using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    public class HealOrbEffect : ItemEffect
    {
        private void Start()
        {
            base.SetUp();
            
        }

        public override void OnPickUp()
        {
            
        }

        public override void Effect()
        {
            ownerPlayerStatus.ChangeHealth(100, "Heal");
        }

    }
}

