using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Scenes.Ingame.Player
{
    public class RecoverMagic : Magic
    {
        [SerializeField] private GameObject _effect;
        public override void ChangeFieldValue()
        {
            chantTime = 15f;
            consumeSanValue = 10;
            Debug.Log("装備している呪文名：RecoverMagic" + "\n装備してる呪文の詠唱時間：" + chantTime + "\n装備してる呪文のSAN値消費量：" + consumeSanValue);
        }

        public override void MagicEffect()
        {
            startTime = Time.time;
            StartCoroutine(Magic());
        }

        private IEnumerator Magic()
        {
            while (true)
            {
                yield return null;

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
                    myPlayerStatus.ChangeHealth(0.5f, "Heal");
                    GameObject effect = Instantiate(Resources.Load<GameObject>("Effect/RecoverEffect/RecoverEffect"), myPlayerStatus.gameObject.transform.position, Quaternion.identity, myPlayerStatus.gameObject.transform);
                    Destroy(effect, effect.GetComponent<VisualEffect>().GetFloat("LifeTime"));

                    //SAN値減少
                    myPlayerStatus.ChangeSanValue(consumeSanValue, "Damage");

                    //呪文を使えないようにする
                    myPlayerMagic.ChangeCanUseMagicBool(false);

                    //成功した詠唱の終了を通知
                    myPlayerMagic.OnPlayerFinishUseMagic.OnNext(default);
                    yield break;
                }
            }
        }
    }

}