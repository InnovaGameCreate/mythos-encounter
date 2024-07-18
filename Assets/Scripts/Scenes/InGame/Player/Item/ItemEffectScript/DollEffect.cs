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
        /// player�̗̑͂�0�ɂȂ����Ƃ��ɌĂяo�����֐�
        /// </summary>
        public void UniqueEffect(PlayerStatus status)
        {
            var PlayerItem = status.gameObject.GetComponent<PlayerItem>();
            //�A�C�e���ɃA�^�b�`����Ă���Effect�n�̃X�N���v�g�Ɏ擾�҂̏��𗬂��B
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
