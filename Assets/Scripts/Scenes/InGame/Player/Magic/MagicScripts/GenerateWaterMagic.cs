using Scenes.Ingame.Enemy;
using System.Collections;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 水を生成する呪文
    /// </summary>
    public class GenerateWaterMagic : Magic
    {
        private EnemyStatus[] _enemyStatuses;
        private GameObject _waterEffect;

        public override void ChangeFieldValue()
        {
            chantTime = 20f;
            consumeSanValue = 10;
            Debug.Log("装備している呪文名：GenerateWaterMagic" + "\n装備してる呪文の詠唱時間：" + chantTime + "\n装備してる呪文のSAN値消費量：" + consumeSanValue);
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
                    //Todo：水エフェクト生成
                    //_waterEffect = Instantiate(Resources.Load<GameObject>(水エフェクトのパス名), myPlayerStatus.gameObject.transform.position, Quaternion.identity, myPlayerStatus.gameObject.transform);

                    _enemyStatuses = FindObjectsOfType<EnemyStatus>();
                    for (int i = 0; i < _enemyStatuses.Length; i++)
                    {
                        _enemyStatuses[i].SetCheckWaterEffect(true);
                    }
                    StartCoroutine(CancelWaterEffect());

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

        private IEnumerator CancelWaterEffect()
        {
            yield return new WaitForSeconds(60f);
            Destroy(_waterEffect);
            Debug.Log("水の影響が無くなります。");
            for (int i = 0; i < _enemyStatuses.Length; i++)
            {
                _enemyStatuses[i].SetCheckWaterEffect(false);
            }
        }
    }
}