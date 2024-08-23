using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class TrapBiteEffect : ItemEffect
    {

        public override void OnPickUp()
        {
            StartCoroutine(InstantTrapBite());
        }

        public override void OnThrow()
        {

        }

        public override void Effect()
        {

        }

        private IEnumerator InstantTrapBite()
        {
            yield break;
        }

    }
}

