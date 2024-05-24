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
    /// �v���C���[�̔����֌W���Ǘ�����X�N���v�g
    /// </summary>
    public class PlayerInsanityManager : MonoBehaviour
    {
        private ReactiveCollection<IInsanity> _insanities = new ReactiveCollection<IInsanity>(); //���݂̔����X�N���v�g���܂Ƃ߂�List
        public IObservable<CollectionAddEvent<IInsanity>> OnInsanitiesAdd => _insanities.ObserveAdd();//�O����__insanities�̗v�f���ǉ����ꂽ�Ƃ��ɍs��������o�^�ł���悤�ɂ���
        public IObservable<CollectionRemoveEvent<IInsanity>> OnInsanitiesRemove => _insanities.ObserveRemove();//�O����__insanities�̗v�f���폜���ꂽ�Ƃ��ɍs��������o�^�ł���悤�ɂ���
        public List<IInsanity> Insanities { get { return _insanities.ToList(); } }//�O����_insanities�̓��e�����J����

        private List<int> _numbers = Enumerable.Range(0, 5).ToList();//0,1,2,3,4�̃��X�g�𐶐�
        /*
         �Ή��\
         0.EyeParalyze
         1.BodyParalyze
         2.IncreasePulsation
         3.Scream
         4.Hallucination
         */

        [SerializeField] private BoolReactiveProperty _isBrainwashed = new BoolReactiveProperty(false);//���]�����ۂ�
        public IObservable<bool> OnPlayerBrainwashedChange { get { return _isBrainwashed; } }//���]��Ԃ��ω������ۂɃC�x���g�����s
        public bool nowPlayerBrainwashed { get { return _isBrainwashed.Value; } }

        private PlayerStatus _myPlayerStatus;
        // Start is called before the first frame update
        void Start()
        {
            _myPlayerStatus = GetComponent<PlayerStatus>();

            //���݂�SAN�l��50�ȉ�����SAN�l�����������ɔ����X�N���v�g��t�^
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

            //�ύX�O��SAN�l��50�ȉ�����SAN�l���񕜂����Ƃ��ɔ�������
            _myPlayerStatus.OnPlayerSanValueChange
                .Where(x => _myPlayerStatus.lastSanValue <= 50 && x > _myPlayerStatus.lastSanValue)
                .Subscribe(x => RecoverInsanity(x / 10 - _myPlayerStatus.lastSanValue / 10))
                .AddTo(this);
        }

        /// <summary>
        /// �����_���Ŕ����X�N���v�g��t�^������ 
        /// </summary>
        /// /// <param name="times">�֐���@����</param>
        private void AddRandomInsanity(int times)
        {
            if (times == 0)
                return;

            for (int i = 0; i < times; i++)
            {
                int random = _numbers[UnityEngine.Random.Range(0, _numbers.Count)];
                //�C�ӂ�IInsanity�֘A�̃X�N���v�g���A�^�b�`
                IInsanity InsanityScript = null;
                switch (random)
                {
                    case 0:
                        InsanityScript = this.AddComponent<EyeParalyze>();
                        _insanities.Add(InsanityScript);
                        break;
                    case 1:
                        InsanityScript = this.AddComponent<BodyParalyze>();
                        _insanities.Add(InsanityScript);
                        break;
                    case 2:
                        InsanityScript = this.AddComponent<IncreasePulsation>();
                        _insanities.Add(InsanityScript);
                        break;
                    case 3:
                        InsanityScript = this.AddComponent<Scream>();
                        _insanities.Add(InsanityScript);
                        break;
                    case 4:
                        InsanityScript = this.AddComponent<Hallucination>();
                        _insanities.Add(InsanityScript);
                        break;
                    default:
                        Debug.Log("�z��O�̒l�ł��B");
                        break;
                }

                //���]��ԂŖ�����Α����ɔ������ʂ𔭊�
                if (InsanityScript != null && !_isBrainwashed.Value)
                {
                    InsanityScript.Active();
                }
                _numbers.Remove(random);
            }           
        }

        /// <summary>
        /// �Ō�ɕt�^���ꂽ�����X�N���v�g����菜��
        /// </summary>
        /// /// /// <param name="times">�֐���@����</param>
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
                        Debug.Log("�z��O�̒l");
                        break;
                }
                _numbers.Sort();

                _insanities.Last().Hide();
                Destroy((UnityEngine.Object)_insanities.Last());
                _insanities.Remove(_insanities.Last());

                //��������ȏ�񕜂���K�v���Ȃ��Ƃ��͏I��
                if (_insanities.Count == 0)
                    break;
            }
        }

        /// <summary>
        /// ���]��ԂɂȂ����ۂɍs���������܂Ƃ߂��R���[�`��
        /// </summary>
        /// <returns></returns>
        public IEnumerator SelfBrainwash()
        {
            //�S�Ă̔����X�N���v�g�𖳌���
            for (int i = 0; i < _insanities.Count; i++)
            {
                _insanities[i].Hide();
            }
            _isBrainwashed.Value = true;

            //���]���ʂ�60�b����
            yield return new WaitForSeconds(60f);

            //�S�Ă̔����X�N���v�g��L���ɂ���
            for (int i = 0; i < _insanities.Count; i++)
            {
                _insanities[i].Active();
            }
            _isBrainwashed.Value = false;
        }

        /// <summary>
        /// ���ݕt�^����Ă��Ȃ������X�N���v�g�̔ԍ����܂Ƃ߂�List���擾�ł���֐�
        /// </summary>
        /// <returns>���ݕt�^����Ă��Ȃ������X�N���v�g�̔ԍ����܂Ƃ߂�List</returns>
        public List<int> GetMyNumbers()
        {
            return _numbers;
        }
    }
}

