using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class HandLIghtEffect : ItemEffect
    {
       
        public override void OnPickUp()
        {
            if (ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.NotActive) //アイテム取得時
            {
                ownerPlayerItem.ActiveHandLight(true);
                ownerPlayerItem.ChangeSwitchHandLight(HandLightState.On);
            }
            else//アイテム選択時
            {
                //以前の状態からライトの起動・停止を決定する
                if (ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.Off)
                {
                    ownerPlayerItem.ActiveHandLight(false);
                }
                else if (ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.On)
                {
                    ownerPlayerItem.ActiveHandLight(true);
                }
            }

            //選択アイテムを別のものにしたとき、自動でライトを停止する
            ownerPlayerItem.OnNowIndexChange
                .Skip(1)
                .Where(x => ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData == null || ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData.itemID != 3)
                .Subscribe(_ =>
                {
                    ownerPlayerItem.ActiveHandLight(false);
                }).AddTo(this);

        }

        public override void OnThrow()
        {
            //アイテム廃棄時にライトを停止する
            ownerPlayerItem.ActiveHandLight(false);
            ownerPlayerItem.ChangeSwitchHandLight(HandLightState.NotActive);
        }

        public override void Effect()
        {
            //左クリック時にライトのON/OFF状態を切り替え、起動・停止する
            SoundManager.Instance.PlaySe("se_switch00", transform.position);
            if (ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.Off)
            {
                ownerPlayerItem.ChangeSwitchHandLight(HandLightState.On);
                ownerPlayerItem.ActiveHandLight(true);
            }
            else if(ownerPlayerItem.SwitchHandLights[ownerPlayerItem.nowIndex] == HandLightState.On)
            {
                ownerPlayerItem.ChangeSwitchHandLight(HandLightState.Off);
                ownerPlayerItem.ActiveHandLight(false);
            }

        }

     
        
    }
}

