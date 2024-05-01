using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    public class BandageEffect : ItemEffect
    {
        private void Start()
        {
            base.SetUp();
        }

        public override void Effect()
        {
            ownerPlayerStatus.ChangeBleedingBool(false);
        }
    }
}
   
