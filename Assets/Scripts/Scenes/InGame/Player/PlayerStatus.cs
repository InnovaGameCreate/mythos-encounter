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
        [SerializeField] private int _speedBase = 5;
        [SerializeField][Tooltip("�v���C���[�̎��Ɩ��̌��̓͂�����")] private float _lightrangeBase = 20;
        [SerializeField][Tooltip("�v���C���[�̂��Ⴊ�ݕ������̉���")] private float _sneakVolumeBase = 5;
        [SerializeField][Tooltip("�v���C���[�̕������̉���")] private float _walkVolumeBase = 10;
        [SerializeField][Tooltip("�v���C���[�̑���̉���")] private float _runVolumeBase = 15;


        //���݂̃X�e�[�^�X�̕ϐ��i����l�b�g���[�N���\��j
        //�������ɂ��Ă�����̓f�[�^�x�[�X���Q�Ƃ��čs���悤�ɂ���B
        [SerializeField] private IntReactiveProperty _health = new IntReactiveProperty();//HP.�[���ɂȂ�Ǝ��S
        [SerializeField] private IntReactiveProperty _stamina = new IntReactiveProperty();//�X�^�~�i
        [SerializeField] private IntReactiveProperty _san = new IntReactiveProperty();//SAN�l
        [SerializeField] private IntReactiveProperty _speed = new IntReactiveProperty();//�ړ����x�̊�l
        [SerializeField] private BoolReactiveProperty _survive = new BoolReactiveProperty(true);//����.true�̂Ƃ��͐����Ă���
        [SerializeField] private BoolReactiveProperty _bleeding = new BoolReactiveProperty(false);//�o�����.true�̂Ƃ��Ɏ��Ԍo�߂ő̗͂�����

        [SerializeField] private ReactiveProperty<PlayerActionState> _playerActionState = new ReactiveProperty<PlayerActionState>(PlayerActionState.Idle);
        [SerializeField] private FloatReactiveProperty _lightrange = new FloatReactiveProperty();//���̓͂�����
        [SerializeField] private FloatReactiveProperty _sneakVolume = new FloatReactiveProperty();//���Ⴊ�ݎ��̉���
        [SerializeField] private FloatReactiveProperty _walkVolume = new FloatReactiveProperty();//���Ⴊ�ݎ��̉���
        [SerializeField] private FloatReactiveProperty _runVolume = new FloatReactiveProperty();//���Ⴊ�ݎ��̉���

        //���̑���Subject
        private Subject<Unit> _enemyAttackedMe = new Subject<Unit>();//�G����U����H������Ƃ��̃C�x���g

        //���ꂼ��̍w�Ǒ������J����B����Class��Subscribe�ł���B
        public IObservable<int> OnPlayerHealthChange { get { return _health; } }//_health(�̗�)���ω������ۂɃC�x���g�����s
        public IObservable<int> OnPlayerStaminaChange { get { return _stamina; } }//_stamina(�X�^�~�i)���ω������ۂɃC�x���g�����s
        public IObservable<int> OnPlayerSanValueChange { get { return _san; } }//_san(SAN�l)���ω������ۂɃC�x���g�����s
        public IObservable<int> OnPlayerSpeedChange { get { return _speed; } }//_speed(�ړ����x�̊�l)���ω������ۂɃC�x���g�����s

        public IObservable<bool> OnPlayerSurviveChange { get { return _survive; } }//_survive(����)���ω������ۂɃC�x���g�����s
        public IObservable<bool> OnPlayerbleedingChange { get { return _bleeding; } }//_bleeding(�o�����)���ω������ۂɃC�x���g�����s
        public IObservable<PlayerActionState> OnPlayerActionStateChange { get { return _playerActionState; } }//_PlayerActionState(�v���C���[�̍s�����)���ω������ۂɃC�x���g�����s
        public IObservable<float> OnLightrangeChange { get { return _lightrange; } }//�v���C���[�̌��̓͂��������ω������ꍇ�ɃC�x���g�����s
        public IObservable<float> OnSneakVolumeChange { get { return _sneakVolume; } }//�v���C���[�̔E�ѕ����̉����͂��������ω������ꍇ�ɃC�x���g�����s
        public IObservable<float> OnWalkVolumeChange { get { return _walkVolume; } }//�v���C���[�̕��������͂��������ω������ꍇ�ɃC�x���g�����s
        public IObservable<float> OnRunVolumeChange { get { return _runVolume; } }//�v���C���[�̑��鉹���͂��������ω������ꍇ�ɃC�x���g�����s

        public IObservable<Unit> OnEnemyAttackedMe { get { return _enemyAttackedMe; } }//�G����U�����󂯂��ۂɃC�x���g�����s

        //�ꕔ���̊J��
        public int playerID { get { return _playerID; } }
        public int stamina_max { get { return _staminaBase; } }
        public int nowStaminaValue { get { return _stamina.Value; } }
        public int nowPlayerSpeed { get { return _speed.Value; } }

        public bool nowBleedingValue { get { return _bleeding.Value; } }
        public PlayerActionState nowPlayerActionState { get { return _playerActionState.Value; } }
        public float nowPlayerLightRange { get { return _lightrange.Value; } }
        public float nowPlayerSneakVolume { get { return _sneakVolume.Value; } }
        public float nowPlayerWalkVolume { get { return _walkVolume.Value; } }
        public float nowPlayerRunVolume { get { return _runVolume.Value; } }

        public int lastHP;//HP�̕ϓ��O�̐��l���L�^�B��r�ɗp����
        public int bleedingDamage = 1;//�o�����Ɏ󂯂�_���[�W
        private bool _isUseItem = false;

        private void Init()
        {
            _health.Value = _healthBase;
            _stamina.Value = _staminaBase;
            _san.Value = _sanBase;
            _speed.Value = _speedBase;
            _lightrange.Value = _lightrangeBase;
            _sneakVolume.Value = _sneakVolumeBase;
            _walkVolume.Value = _walkVolumeBase;
            _runVolume.Value = _runVolumeBase;
        }
        // Start is called before the first frame update
        void Awake()
        {
            //������
            Init();
            _health.Subscribe(x => CheckHealth(x,_playerID));//�̗͂��ω������Ƃ��ɃQ�[�����ŕύX��������
            _stamina.Subscribe(x => CheckStamina(x, _playerID));//�X�^�~�i���ω������Ƃ��ɃQ�[�����ŕύX��������
            _san.Subscribe(x => CheckSanValue(x, _playerID));//SAN�l���ω������Ƃ��ɃQ�[�����ŕύX��������
            _bleeding.
                Where(x => x == true).
                Subscribe(_ => StartCoroutine(Bleeding(bleedingDamage)));//�o����ԂɂȂ����Ƃ��ɏo���������J�n
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //�f�o�b�O�p.(�K�v�����Ȃ�Ώ���)
            if (Input.GetKeyDown(KeyCode.L))
            {
                ChangeHealth(20, "Damage");
                ChangeSanValue(20, "Damage");
                ChangeBleedingBool(true);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                ChangeBleedingBool(false);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                _enemyAttackedMe.OnNext(default);
            }
#endif           
        }

        /// <summary>
        /// �̗͂�ύX�����邽�߂̊֐�
        /// </summary>
        /// <param name="value">�ύX��</param>
        /// <param name="mode">Heal(��), Damage(����)�̓�̂�</param>
        public void ChangeHealth(int value, string mode)
        {
            if (mode == "Heal")
            {
                lastHP = _health.Value;
                _health.Value = Mathf.Min(100, _health.Value + value);
            }
            else if (mode == "Damage")
            {
                lastHP = _health.Value;
                _health.Value = Mathf.Max(0, _health.Value - value);
            }
        }

        /// <summary>
        /// �X�^�~�i��ύX�����邽�߂̊֐�
        /// </summary>
        /// <param name="value">�ύX��</param>
        /// <param name="mode">Heal(��), Damage(����)�̓�̂�</param>
        public void ChangeStamina(int value, string mode)
        {
            if (mode == "Heal")
                _stamina.Value = Mathf.Min(100, _stamina.Value + value);
            else if (mode == "Damage")
                _stamina.Value = Mathf.Max(0, _stamina.Value - value);
        }

        /// <summary>
        /// SAN�l��ύX�����邽�߂̊֐�
        /// </summary>
        /// <param name="value">�ύX��</param>
        /// <param name="mode">Heal(��), Damage(����)�̓�̂�</param>
        public void ChangeSanValue(int value, string mode)
        {
            if (mode == "Heal")
                _san.Value = Mathf.Min(100, _san.Value + value);
            else if (mode == "Damage")
                _san.Value = Mathf.Max(0, _san.Value - value);
        }

        /// <summary>
        /// �ړ����x��ύX������֐�
        /// </summary>
        public void ChangeSpeed()
        {
            _speed.Value = (int)(_speedBase * (_isUseItem ? 0.5f : 1));
        }

        /// <summary>
        /// �A�C�e�����g���Ă���̂��Ǘ����邽�߂̊֐�
        /// </summary>
        /// <param name="value"></param>
        public void UseItem(bool value)
        { 
            _isUseItem = value;
        }

        /// <summary>
        /// _bleeding(�o�����)�̒l��ύX���邽�߂̊֐�
        /// </summary>
        /// <param name="value"></param>
        public void ChangeBleedingBool(bool value)
        {
            _bleeding.Value = value;
        }

        public void ChangePlayerActionState(PlayerActionState state)
        {
            _playerActionState.Value = state;
        }

        /// <summary>
        /// �o����Ԃ̏������s���֐��B
        /// </summary>
        /// <returns></returns>
        private IEnumerator Bleeding(int damage)
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                if (_bleeding.Value)
                { 
                    ChangeHealth(damage, "Damage");
                    Debug.Log("�o���_���[�W������܂����B");
                }                   
                else 
                    yield break;
            }
        }

        /// <summary>
        /// �̗͂Ɋւ��鏈�����s��
        /// </summary>
        /// <param name="health">�c��̗�</param>
        private void CheckHealth(int health, int ID)
        {
            //Debug.Log("�c��̗́F" + health);

            
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
            //Debug.Log("�c��X�^�~�i�F" + stamina);
        }

        /// <summary>
        /// san�l�Ɋւ��鏈�����s��
        /// </summary>
        /// <param name="san">�c���SAN�l</param>
        private void CheckSanValue(int sanValue, int ID)
        {
            //Debug.Log("�c��san�l�F" + sanValue);

            if (sanValue <= 0)
                _survive.Value = false;
        }
    }
}

