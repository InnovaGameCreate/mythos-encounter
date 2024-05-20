using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// プレイヤーの発狂関係を管理するスクリプト
    /// </summary>
    public class PlayerInsanityManager : MonoBehaviour
    {
        private ReactiveCollection<IInsanity> _insanities = new ReactiveCollection<IInsanity>(); //現在の発狂スクリプトをまとめたList
        public IObservable<CollectionAddEvent<IInsanity>> OnInsanitiesAdd => _insanities.ObserveAdd();//外部に__insanitiesの要素が追加されたときに行う処理を登録できるようにする
        public IObservable<CollectionRemoveEvent<IInsanity>> OnInsanitiesRemove => _insanities.ObserveRemove();//外部に__insanitiesの要素が削除されたときに行う処理を登録できるようにする
        public List<IInsanity> Insanities { get { return _insanities.ToList(); } }//外部に_insanitiesの内容を公開する

        private List<int> _numbers = Enumerable.Range(0, 5).ToList();//0,1,2,3,4のリストを生成

        private PlayerStatus _myPlayerStatus;
        // Start is called before the first frame update
        void Start()
        {
            _myPlayerStatus = GetComponent<PlayerStatus>();

            //現在のSAN値が50以下かつSAN値が減った時に発狂スクリプトを付与
            _myPlayerStatus.OnPlayerSanValueChange
                .Where(x => x <= 50 && x < _myPlayerStatus.lastSanValue)
                .Subscribe(x =>
                {
                    if(40 < x && x <= 50)
                        AddRandomInsanity(1 - _insanities.Count);

                    else if (30 < x && x <= 40)
                        AddRandomInsanity(2 - _insanities.Count);

                    else if (20 < x && x <= 30)
                        AddRandomInsanity(3 - _insanities.Count);

                    else if (10 < x && x <= 20)
                        AddRandomInsanity(4 - _insanities.Count);

                    else if (0 < x && x <= 10)
                        AddRandomInsanity(5 - _insanities.Count);

                }).AddTo(this);

            //変更前のSAN値が50以下かつSAN値が回復したときに発狂を回復
            _myPlayerStatus.OnPlayerSanValueChange
                .Where(x => _myPlayerStatus.lastSanValue <= 50 && x > _myPlayerStatus.lastSanValue)
                .Subscribe(x => RecoverInsanity(x / 10 - _myPlayerStatus.lastSanValue / 10))
                .AddTo(this);
        }

        /// <summary>
        /// ランダムで発狂スクリプトを付与させる 
        /// </summary>
        /// /// <param name="times">関数を叩く回数</param>
        private void AddRandomInsanity(int times)
        {
            if (times == 0)
                return;

            for (int i = 0; i < times; i++)
            {
                int random = _numbers[UnityEngine.Random.Range(0, _numbers.Count)];
                //任意のIInsanity関連のスクリプトをアタッチ
                IInsanity InsanityScript;
                switch (random)
                {
                    case 0:
                        InsanityScript = this.AddComponent<EyeParalyze>();
                        _insanities.Add(InsanityScript);
                        InsanityScript.Active();//アタッチしたスクリプトを発動
                        break;
                    case 1:
                        InsanityScript = this.AddComponent<BodyParalyze>();
                        _insanities.Add(InsanityScript);
                        InsanityScript.Active();
                        break;
                    case 2:
                        InsanityScript = this.AddComponent<IncreasePulsation>();
                        _insanities.Add(InsanityScript);
                        InsanityScript.Active();
                        break;
                    case 3:
                        InsanityScript = this.AddComponent<Scream>();
                        _insanities.Add(InsanityScript);
                        InsanityScript.Active();
                        break;
                    case 4:
                        InsanityScript = this.AddComponent<Hallucination>();
                        _insanities.Add(InsanityScript);
                        InsanityScript.Active();
                        break;
                    default:
                        Debug.Log("想定外の値です。");
                        break;
                }
                _numbers.Remove(random);
            }           
        }

        /// <summary>
        /// 最後に付与された発狂スクリプトを取り除く
        /// </summary>
        /// /// /// <param name="times">関数を叩く回数</param>
        private void RecoverInsanity(int times)
        {
            if (times == 0)
                return;

            for (int i = 0; i < times; i++)
            {
                switch (_insanities.Last())
                {
                    case EyeParalyze:
                        _numbers.Add(0);
                        break;
                    case BodyParalyze:
                        _numbers.Add(1);
                        break;
                    case IncreasePulsation:
                        _numbers.Add(2);
                        break;
                    case Scream:
                        _numbers.Add(3);
                        break;
                    case Hallucination:
                        _numbers.Add(4);
                        break;
                    default:
                        Debug.Log("想定外の値");
                        break;
                }
                _numbers.Sort();

                _insanities.Last().Hide();
                _insanities.Remove(_insanities.Last());

                //もうこれ以上回復する必要がないときは終了
                if (_insanities.Count == 0)
                    break;
            }
        }

        /// <summary>
        /// 現在付与されていない発狂スクリプトの番号をまとめたListを取得できる関数
        /// </summary>
        /// <returns>現在付与されていない発狂スクリプトの番号をまとめたList</returns>
        public List<int> GetMyNumbers()
        {
            return _numbers;
        }
    }
}

