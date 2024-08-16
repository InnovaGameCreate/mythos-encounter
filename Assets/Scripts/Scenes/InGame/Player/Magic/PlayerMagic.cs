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
        private bool _isUsedMagic = false;//魔法を1度使ったか否か
        [SerializeField] private Magic _myMagic;//使用可能な魔法

        private Subject<Unit> _FinishUseMagic = new Subject<Unit>();//魔法の詠唱が終わり、効果が発動したらイベントが発生.
        public IObserver<Unit> OnPlayerFinishUseMagic { get { return _FinishUseMagic; } }//外部で_FinishUseMagicのOnNextを呼ぶためにIObserverを公開

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
            //100ms以内では中断不可
            this.UpdateAsObservable()
                .Where(_ => _isCanUseMagic && Input.GetKeyDown(KeyCode.Q))
                .ThrottleFirst(TimeSpan.FromMilliseconds(100))
                .Subscribe(_ =>
                {
                    if (myPlayerStatus.nowPlayerUseMagic)//呪文を詠唱していたら
                    {
                        //詠唱中の移動速度50%Downを解除
                        myPlayerStatus.UseMagic(false);
                        myPlayerStatus.ChangeSpeed();

                        //魔法を使う処理をキャンセル
                        _myMagic.cancelMagic = true;
                        Debug.Log("操作による詠唱中止");
                    }
                    else//呪文をまだ詠唱していないとき
                    {
                        //San値が10以下のときは詠唱できない
                        if (myPlayerStatus.nowPlayerSanValue <= _myMagic.consumeSanValue)
                        {
                            Debug.Log("SAN値が足りないので詠唱できません");
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
                                    Debug.Log("発狂していないので呪文を使う必要がありません");
                                }
                                break;
                            case RecoverMagic:
                                if (myPlayerStatus.nowPlayerHealth == myPlayerStatus.health_max)
                                {
                                    needMagic = false;
                                    Debug.Log("体力減っていないので呪文を使う必要がありません");
                                }
                                break;
                            default:
                                break;
                        }

                        if (needMagic)
                        {
                            //詠唱中は移動速度50%Down
                            myPlayerStatus.UseMagic(true);
                            myPlayerStatus.ChangeSpeed();

                            //魔法を使う処理
                            _myMagic.MagicEffect();
                            Debug.Log("呪文の詠唱開始");
                        }
                    }
                });

            //攻撃くらったときのイベントが発行されたときに呪文詠唱を中断
            myPlayerStatus.OnEnemyAttackedMe
                .Where(_ => _isCanUseMagic)
                .Subscribe(_ =>
                {
                    //詠唱中の移動速度50%Downを解除
                    myPlayerStatus.UseMagic(false);
                    myPlayerStatus.ChangeSpeed();

                    //魔法を使う処理をキャンセル
                    _myMagic.cancelMagic = true;
                    Debug.Log("攻撃を受けたので詠唱中止！");
                }).AddTo(this);


            //呪文の詠唱が終了したら足の遅さを元に戻す。
            _FinishUseMagic
                .Subscribe(_ =>
                {
                    //詠唱中の移動速度50%Downを解除
                    myPlayerStatus.UseMagic(false);
                    myPlayerStatus.ChangeSpeed();
                    _isUsedMagic = true;
                }).AddTo(this);
        }

        public void ChangeCanUseMagicBool(bool value)
        {
            _isCanUseMagic = false;
        }

        /// <summary>
        /// 既に１度呪文を使ったかを管理しているBoolの値を取得する関数
        /// </summary>
        /// <returns>_isUsedMagicの値</returns>
        public bool GetUsedMagicBool()
        { 
            return _isUsedMagic;
        }
    }
}

