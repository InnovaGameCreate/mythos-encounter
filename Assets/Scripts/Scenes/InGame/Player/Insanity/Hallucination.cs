using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Runtime.CompilerServices;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 発狂の種類の1つ
    /// 1.たまに存在しない足音が聞こえる
    /// 2.たまに存在する足音が聞こえなくなる
    /// 3.たまに存在しない生物が見える(点滅をする)
    /// 4.これらを3分に一度ランダムで付与（プレイヤーはどの効果がつくか体験するまでわからない）
    /// </summary>
    public class Hallucination : MonoBehaviour,IInsanity
    {
        private PlayerSoundManager _soundManager;
        private AudioListener _audioListener;
        private GameObject _HallucinationPrefab;//幻覚用のGameObjectのプレハブ

        private ReactiveProperty<bool> _decideCoroutineBool = new ReactiveProperty<bool>(true);
        public void Setup()
        {
            _soundManager = GetComponent<PlayerSoundManager>();
            _audioListener = transform.Find("Main Camera").gameObject.GetComponent<AudioListener>();
            //_HallucinationPrefab = Resources.Load();

            _decideCoroutineBool
                .Where(x => x == true)
                .Subscribe(x => DecideCoroutine()).AddTo(this);
        }

        public void Active()
        {
            Setup();

        }

        public void Hide()
        {
            Destroy(this);
        }

        /// <summary>
        /// どの効果を付与するか決定する関数
        /// </summary>
        private void DecideCoroutine()
        {
            int random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    StartCoroutine(HearingHallucination());
                    Debug.Log("幻聴が聞こえ始める");
                    break;
                case 1:
                    StartCoroutine(AppearHallucination());
                    Debug.Log("幻覚が見え始める");
                    break;
                case 2:
                    StartCoroutine(TemporarilyLostHearing());
                    Debug.Log("たまに聴力がなくなる");
                    break;
                default:
                    Debug.Log("想定外の数値です");
                    break;
            }

            _decideCoroutineBool.Value = false;
        }

        /// <summary>
        /// 幻聴が聞こえるようになるコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator HearingHallucination()
        {
            //3分間続ける
            for (int i = 0; i < 18; i++)
            {
                int random = Random.Range(0, 5);

                //20%の確率で幻聴が聞こえる
                if (random == 0)
                { 
                    //幻聴用のクリップを流す。
                }
                yield return new WaitForSeconds(10.0f);
            }

            _decideCoroutineBool.Value = true;
        }

        /// <summary>
        /// 幻覚が見えるようになるコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator AppearHallucination()
        {
            GameObject HallucinationObject;//幻覚を格納する変数
            //3分間続ける
            for (int i = 0; i < 18; i++)
            {
                int random = Random.Range(0, 5);

                //20%の確率で幻覚が見えるようになる
                if (random == 0)
                {
                    //幻覚を出現させる
                }
                else
                { 
                    //幻覚を破壊する
                }
                yield return new WaitForSeconds(10.0f);
            }

            _decideCoroutineBool.Value = true;
        }

        /// <summary>
        /// たまに聴力を失って音が聞こえなくなるコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator TemporarilyLostHearing()
        {
            //3分間続ける
            for (int i = 0; i < 18; i++)
            {
                int random = Random.Range(0, 5);

                //20%の確率で耳が聞こえなくなる
                if (random == 0)
                {
                    _audioListener.enabled = false;
                }
                else
                {
                    _audioListener.enabled = true;
                }
                yield return new WaitForSeconds(10.0f);
            }

            _decideCoroutineBool.Value = true;
        }
    }
}
