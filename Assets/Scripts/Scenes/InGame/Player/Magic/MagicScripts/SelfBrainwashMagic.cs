using Scenes.Ingame.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 自身に洗脳をかけて一時的に正気に戻り，状態異常を無効化する呪文
    /// 効果時間 60秒
    /// </summary>
    public class SelfBrainwashMagic : Magic
    {
        private PlayerInsanityManager _myPlayerInsanityManager;
        private bool _debugMode = false;
        public override void ChangeFieldValue()
        {
            chantTime = 20f;
            consumeSanValue = 10;
            Debug.Log("装備している呪文名：SelfBrainwashMagic" + "\n装備してる呪文の詠唱時間：" + chantTime + "\n装備してる呪文のSAN値消費量：" + consumeSanValue);
        }

        public override void MagicEffect()
        {
            startTime = Time.time;
            StartCoroutine(Magic());
        }

        private IEnumerator Magic()
        {
            _myPlayerInsanityManager = this.GetComponent<PlayerInsanityManager>();
            while (true)
            {
                yield return null;

                if(_debugMode)
                    Debug.Log(Time.time - startTime);

                //攻撃を食らった際にこのコルーチンを破棄              
                if (cancelMagic == true)
                {
                    cancelMagic = false;
                    yield break;
                }

                //呪文発動
                if (Time.time - startTime >= chantTime)
                {
                    Debug.Log("呪文発動！");
                    //効果発動
                    StartCoroutine(_myPlayerInsanityManager.SelfBrainwash());

                    //SAN値減少
                    myPlayerStatus.ChangeSanValue(consumeSanValue, "Damage");

                    //呪文を使えないようにする
                    myPlayerMagic.ChangeCanUseMagicBool(false);
                    yield break;
                }
            }
        }
    }
}
