using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    public class HandLIghtEffect : ItemEffect
    {
        


        private void Start()
        {
            if(ownerPlayerItem != null)
            {
                //アイテム選択時にライトの起動・停止を保存されているON/OFF状態から決定する
                ownerPlayerItem.ActiveHandLight(ownerPlayerItem.SwitchHandLight);


            }
        }

        public override void OnPickUp()
        {
            //アイテム取得時にライトを起動する
            ownerPlayerItem.ActiveHandLight(true);
            if (!ownerPlayerItem.SwitchHandLight)
            {
                ownerPlayerItem.ChangeSwitchHandLight();
            }

            ownerPlayerItem.OnNowIndexChange
                .Skip(1)
                .Subscribe(_ =>
                {
                    ownerPlayerItem.ActiveHandLight(false);
                });

        }

        public override void OnThrow()
        {
            //アイテム廃棄時にライトを停止する
            ownerPlayerItem.ActiveHandLight(false);
        }

        public override void Effect()
        {
            //左クリック時にライトのON/OFF状態を切り替え、起動・停止する
            ownerPlayerItem.ChangeSwitchHandLight();
            ownerPlayerItem.ActiveHandLight(ownerPlayerItem.SwitchHandLight);
            Debug.Log($"SwitchHandLIghtは{ownerPlayerItem.SwitchHandLight}");
        }

     
        
    }
}

