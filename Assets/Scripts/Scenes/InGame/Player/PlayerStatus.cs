using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// �v���C���[�̃X�e�[�^�X���Ǘ�����N���X
/// MV(R)P�ɂ�����Model�̖�����z��
/// </summary>
namespace Scenes.Ingame.Player
{

    public class PlayerStatus : MonoBehaviour
    {
        //�v���C���[�̃f�[�^�x�[�X(���u��)
        [Header("�v���[���[�̃f�[�^�x�[�X")]
        [SerializeField] private int _playerID = 0;
        [SerializeField] private int _healthBase = 100;
        [SerializeField] private int _staminaBase = 100;
        [SerializeField] private int _sanBase = 100;

        public int playerID { get { return _playerID; } }

        //���݂̃X�e�[�^�X�̕ϐ��i����l�b�g���[�N���\��j
        //�������ɂ��Ă�����̓f�[�^�x�[�X���Q�Ƃ��čs���悤�ɂ���B
        [SerializeField] private IntReactiveProperty _health = new IntReactiveProperty();//HP.�[���ɂȂ�Ǝ��S
        [SerializeField] private IntReactiveProperty _stamina = new IntReactiveProperty();//�X�^�~�i
        [SerializeField] private IntReactiveProperty _san = new IntReactiveProperty();//SAN�l
        [SerializeField] private BoolReactiveProperty _survive = new BoolReactiveProperty(true);//����.true�̂Ƃ��͐����Ă���
        [SerializeField] private BoolReactiveProperty _bleeding = new BoolReactiveProperty(false);//�o�����.true�̂Ƃ��Ɏ��Ԍo�߂ő̗͂�����


        //���ꂼ��̍w�Ǒ������J����B����Class��Subscribe�ł���B
        public IObservable<int> OnPlayerHealthChange { get { return _health; } }//_health(�̗�)���ω������ۂɃC�x���g�����s
        public IObservable<int> OnPlayerStaminaChange { get { return _stamina; } }//_stamina(�X�^�~�i)���ω������ۂɃC�x���g�����s
        public IObservable<int> OnPlayerSanValueChange { get { return _san; } }//_san(SAN�l)���ω������ۂɃC�x���g�����s
        public IObservable<bool> OnPlayerSurviveChange { get { return _survive; } }//_survive(����)���ω������ۂɃC�x���g�����s
        public IObservable<bool> OnPlayerbleedingChange { get { return _bleeding; } }//_bleeding(�o�����)���ω������ۂɃC�x���g�����s


        //[SerializeField] private DisplayPlayerStatusManager _displayPlayerStatusManager;

        private void Init()
        {
            _health.Value = _healthBase;
            _stamina.Value = _staminaBase;
            _san.Value = _sanBase;
        }
        // Start is called before the first frame update
        void Start()
        {
            //������
            Init();
            _health.Subscribe(x => CheckHealth(x,_playerID));//�̗͂��ω������Ƃ��ɃQ�[�����ŕύX��������
            _stamina.Subscribe(x => CheckStamina(x, _playerID));//�X�^�~�i���ω������Ƃ��ɃQ�[�����ŕύX��������
            _san.Subscribe(x => CheckSanValue(x, _playerID));//SAN�l���ω������Ƃ��ɃQ�[�����ŕύX��������
            _bleeding.
                Where(x => x == true).
                Subscribe(_ => StartCoroutine(Bleeding()));//�o����ԂɂȂ����Ƃ��ɏo���������J�n
        }

        // Update is called once per frame
        void Update()
        {
            //�f�o�b�O�p.
            if (Input.GetKeyDown(KeyCode.L))
            {
                ChangeHealth(20, "damage");
                ChangeStamina(20, "damage");
                ChangeSanValue(20, "damage");
                ChangeBleedingBool(true);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                ChangeBleedingBool(false);
            }
        }

        /// <summary>
        /// �̗͂�ύX�����邽�߂̊֐�
        /// </summary>
        /// <param name="value">�ύX��</param>
        /// <param name="mode">Heal(��), damage(����)�̓�̂�</param>
        public void ChangeHealth(int value, string mode)
        {
            if (mode == "Heal")
                _health.Value = Mathf.Min(100, _health.Value + value); 

            else if(mode == "damage")
                _health.Value = Mathf.Max(0, _health.Value - value);

        }

        /// <summary>
        /// �X�^�~�i��ύX�����邽�߂̊֐�
        /// </summary>
        /// <param name="value">�ύX��</param>
        /// <param name="mode">Heal(��), damage(����)�̓�̂�</param>
        public void ChangeStamina(int value, string mode)
        {
            if (mode == "Heal")
                _stamina.Value = Mathf.Min(100, _stamina.Value + value);
            else if (mode == "damage")
                _stamina.Value = Mathf.Max(0, _stamina.Value - value);
        }

        /// <summary>
        /// SAN�l��ύX�����邽�߂̊֐�
        /// </summary>
        /// <param name="value">�ύX��</param>
        /// <param name="mode">Heal(��), damage(����)�̓�̂�</param>
        public void ChangeSanValue(int value, string mode)
        {
            if (mode == "Heal")
                _san.Value = Mathf.Min(100, _san.Value + value);
            else if (mode == "damage")
                _san.Value = Mathf.Max(0, _san.Value - value);
        }

        /// <summary>
        /// _bleeding(�o�����)�̒l��ύX���邽�߂̊֐�
        /// </summary>
        /// <param name="value"></param>
        public void ChangeBleedingBool(bool value)
        {
            _bleeding.Value = value;
        }

        /// <summary>
        /// �o����Ԃ̏������s���֐��B
        /// </summary>
        /// <returns></returns>
        private IEnumerator Bleeding()
        {
            while (_bleeding.Value)
            {
                yield return new WaitForSeconds(1.0f);
                if (_bleeding.Value) 
                    ChangeHealth(1, "damage");
                else 
                    break;
            }
        }

        /// <summary>
        /// �̗͂Ɋւ��鏈�����s��
        /// </summary>
        /// <param name="health">�c��̗�</param>
        private void CheckHealth(int health, int ID)
        {
            Debug.Log("�c��̗́F" + health);

            
            if (health <= 0)
                _survive.Value = false;
        }

        /// <summary>
        /// �X�^�~�i�Ɋւ��鏈�����s��
        /// </summary>
        /// <param name="stamina">�c��X�^�~�i</param>
        private void CheckStamina(int stamina, int ID)
        {
            //�X�^�~�i�c�ʂ��Q�[�����ɕ\��.
            Debug.Log("�c��X�^�~�i�F" + stamina);
        }

        /// <summary>
        /// san�l�Ɋւ��鏈�����s��
        /// </summary>
        /// <param name="san">�c���SAN�l</param>
        private void CheckSanValue(int sanValue, int ID)
        {
            Debug.Log("�c��san�l�F" + sanValue);

            if (sanValue <= 0)
                _survive.Value = false;
        }


    }
}

