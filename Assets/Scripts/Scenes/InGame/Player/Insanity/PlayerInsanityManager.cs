using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �v���C���[�̔����֌W���Ǘ�����X�N���v�g
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
        /// �����_���Ŕ����ɂ��f�o�t��t�^������ 
        /// </summary>
        public void AddRandomInsanity()
        {
            List<int> numbers = Enumerable.Range(1,5).ToList();

            //���ڈȍ~
            if (_historys.FirstOrDefault() != 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < _historys.Count; j++)
                    {
                        //���ɏo�Ă���f�o�t�ԍ��ł���Ώ����i�����f�o�t��t�^���Ȃ����߁j
                        if (numbers[i] == _historys[j])
                        { 
                            numbers.Remove(i);
                            break;
                        }                          
                    }
                }
            }

            int random = numbers[Random.Range(0, numbers.Count)];
            //�C�ӂ�IInsanity�֘A�̃X�N���v�g���A�^�b�`
            IInsanity InsanityScript;
            switch (random) 
            {
                case 1:
                    InsanityScript = this.AddComponent<EyeParalyze>();
                    _insanities.Add(InsanityScript);
                    InsanityScript.Active();//�A�^�b�`�����X�N���v�g�𔭓�
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
                    Debug.Log("�z��O�̒l�ł��B");
                    break;
            }
            _historys.Add(random);

        }
    }
}

