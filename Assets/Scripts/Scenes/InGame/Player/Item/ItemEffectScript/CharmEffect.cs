using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class CharmEffect : ItemEffect
    {
        private bool _isHaveCharm = false;
        public override void OnPickUp()
        {
            ownerPlayerStatus.HaveCharm(true);
        }

        public override void OnThrow()
        {
            //�@�A�C�e���X���b�g���ɂ܂�����肪�c���Ă��邩�ǂ�������
            for (int i = 0 ; i < 7 ; i++) {
                var item = ownerPlayerItem.ItemSlots[i];
                if ( i != ownerPlayerItem.nowIndex) {
                    if (item.myItemData != null) {
                        if (item.myItemData.itemID == 6) {
                            _isHaveCharm = true;
                        }
                    }
                }
            }
            //����肪�c���Ă��Ȃ���Ό��ʏ���
            if (!_isHaveCharm) {
                ownerPlayerStatus.HaveCharm(false);
            }          
        }

        public override void Effect()
        {

        }

      
    }
}

