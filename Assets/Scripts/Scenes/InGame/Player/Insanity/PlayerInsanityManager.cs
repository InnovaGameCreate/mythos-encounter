using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// プレイヤーの発狂関係を管理するスクリプト
    /// </summary>
    public class PlayerInsanityManager : MonoBehaviour
    {
        private List<int> _historys= new List<int>();
        private List<IInsanity> _insanities = new List<IInsanity>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// ランダムで発狂によるデバフを付与させる 
        /// </summary>
        public void AddRandomInsanity()
        {
            List<int> numbers = Enumerable.Range(1,5).ToList();

            //二回目以降
            if (_historys.FirstOrDefault() != 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < _historys.Count; j++)
                    {
                        //既に出ているデバフ番号であれば除去（同じデバフを付与しないため）
                        if (numbers[i] == _historys[j])
                        { 
                            numbers.Remove(i);
                            break;
                        }                          
                    }
                }
            }

            int random = numbers[Random.Range(0, numbers.Count)];
            //任意のIInsanity関連のスクリプトをアタッチ
            IInsanity InsanityScript;
            switch (random) 
            {
                case 1:
                    InsanityScript = this.AddComponent<EyeParalyze>();
                    _insanities.Add(InsanityScript);
                    InsanityScript.Active();//アタッチしたスクリプトを発動
                    break;
                case 2:
                    InsanityScript = this.AddComponent<BodyParalyze>();
                    _insanities.Add(InsanityScript);
                    InsanityScript.Active();
                    break;
                case 3:
                    InsanityScript = this.AddComponent<IncreasePulsation>();
                    _insanities.Add(InsanityScript);
                    InsanityScript.Active();
                    break;
                case 4:
                    InsanityScript = this.AddComponent<Scream>();
                    _insanities.Add(InsanityScript);
                    InsanityScript.Active();
                    break;
                case 5:
                    InsanityScript = this.AddComponent<Hallucination>();
                    _insanities.Add(InsanityScript);
                    InsanityScript.Active();
                    break;
                default:
                    Debug.Log("想定外の値です。");
                    break;
            }
            _historys.Add(random);

        }
    }
}

