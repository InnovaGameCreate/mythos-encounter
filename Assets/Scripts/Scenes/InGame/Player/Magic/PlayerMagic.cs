using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

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

            //呪文スクリプトにPlayerStatusとPlayerMagicを取得させる
            _myMagic.myPlayerStatus = myPlayerStatus;
            _myMagic.myPlayerMagic = this;

            //Qキーで呪文の詠唱を開始 or 中止させる処理
            this.UpdateAsObservable()
                .Where(_ => _isCanUseMagic && Input.GetKeyDown(KeyCode.Q))
                .ThrottleFirst(TimeSpan.FromMilliseconds(100))
                .Subscribe(_ =>
                {
                    if (myPlayerStatus.nowPlayerUseMagic)//呪文を詠唱していたら
                    {
                        //詠唱中の移動速度50%Downを解除
                        myPlayerStatus.UseMagic(false);

                        //魔法を使う処理をキャンセル
                        _myMagic.cancelMagic = true;
                        Debug.Log("操作による詠唱中止");
                    }
                    else//呪文をまだ詠唱していないとき
                    {
                        //San値が10以下のときは詠唱できない
                        if (myPlayerStatus.nowPlayerSanValue <= 10)
                        {
                            Debug.Log("SAN値が10以下なので詠唱できません");
                            return;
                        }

                        //各呪文で一部使用しなくて良い状況であれば呪文を使わせない
                        bool needMagic = true;//呪文を使う必要があるか否か
                        switch (_myMagic)
                        {
                            case SelfBrainwashMagic:
                                if (myPlayerStatus.nowPlayerSanValue > 50)
                                {
                                    needMagic = false;
                                    Debug.Log("呪文を使う必要がありません");
                                }
                                break;
                            default: 
                                break;
                        }

                        if (needMagic)
                        {
                            //詠唱中は移動速度50%Down
                            myPlayerStatus.UseMagic(true);

                            //魔法を使う処理
                            _myMagic.MagicEffect();
                            Debug.Log("呪文の詠唱開始");
                        }
                    }                   
                });

            //攻撃くらったときを示すBoolがTrueになったときに呪文詠唱を中断
            myPlayerStatus.OnEnemyAttackedMe
                .Where(_ => _isCanUseMagic)
                .Subscribe(_ =>
                {
                    //詠唱中の移動速度50%Downを解除
                    myPlayerStatus.UseMagic(false);

                    //魔法を使う処理をキャンセル
                    _myMagic.cancelMagic = true;
                    Debug.Log("攻撃を受けたので詠唱中止！");
                }).AddTo(this);
        }

        public void ChangeCanUseMagicBool(bool value)
        {
            _isCanUseMagic = false;
        }
    }
}

