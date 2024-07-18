using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scenes.Ingame.Enemy;
using UniRx;
using System;

namespace Scenes.Ingame.Player
{
    public class DollEffect : ItemEffect
    {
        public override void OnPickUp()
        {
            ownerPlayerStatus.ChangeCountDoll(1);
        }

        public override void OnThrow()
        {
            ownerPlayerStatus.ChangeCountDoll(-1);
        }

        public override void Effect()
        {

        }

        /// <summary>
        /// playerの体力が0になったときに呼び出される関数
        /// </summary>
        public void UniqueEffect(PlayerStatus status)
        {
            var PlayerItem = status.gameObject.GetComponent<PlayerItem>();
            //アイテムにアタッチされているEffect系のスクリプトに取得者の情報を流す。
            ownerPlayerStatus = status;
            ownerPlayerItem = PlayerItem;
            ownerPlayerStatus.ReviveCharacter(); 
            if(GameObject.FindWithTag("Enemy") != null)
            {
                GameObject.FindWithTag("Enemy").GetComponent<EnemyMove>().ResetPosition();
            }
            ownerPlayerStatus.ChangeCountDoll(-1);

        }




    }
}
