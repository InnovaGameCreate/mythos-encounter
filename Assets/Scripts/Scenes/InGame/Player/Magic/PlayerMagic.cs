using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// プレイヤーの魔法関連を管理するスクリプト
    /// </summary>
    public class PlayerMagic : MonoBehaviour
    {
        private bool _isCanUseMagic = true;//現在魔法が使えるか否か
        [SerializeField] private Magic _myMagic;//使用可能な魔法
        public void Start()
        {
            //自身のPlayerStatusを取得
            PlayerStatus myPlayerStatus = this.GetComponent<PlayerStatus>();

            //_myMagicの中身を自身が設定した呪文に設定する処理
            //α版では無視(インゲーム前が実装されたら実装)

            //呪文を使う処理
            this.UpdateAsObservable()
                .Where(_ => _isCanUseMagic == true && Input.GetKeyDown(KeyCode.Q))
                .Subscribe(_ =>
                {
                    //魔法を使う処理とSAN値減少処理
                    _myMagic.MagicEffect();
                    myPlayerStatus.ChangeSanValue(_myMagic.consumeSanValue, "Damage");

                    //クールタイム開始
                    StartCoroutine(MagicCoolTime(_myMagic.coolTime));
                });
        }


        private IEnumerator MagicCoolTime(float coolTime)
        {
            _isCanUseMagic = false;
            yield return new WaitForSeconds(coolTime);
            _isCanUseMagic = true;
            Debug.Log("呪文クールタイム終了");
        }
    }
}

