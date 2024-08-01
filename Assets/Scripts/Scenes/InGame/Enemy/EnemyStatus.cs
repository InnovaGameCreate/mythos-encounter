using Scenes.Ingame.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using Scenes.Ingame.InGameSystem;
using System;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine.AI;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// �G�̃X�y�b�N�ƌ��݂̏�Ԃ��L�^����
    /// </summary>
    public class EnemyStatus : MonoBehaviour
    {
        //Count�̓^�C�~���O���v��悤�ȕϐ���\���B�Ⴆ��..�N�[���_�E�����ǂꂾ���I�����Ă��邩�≽�����U�����������Ȃ�
        [Header("�G�L�����̊�{�X�y�b�N�̏����l")]
        [SerializeField][Tooltip("hp�̏����l")] private int _hpBase;
        [SerializeField][Tooltip("���񎞂̑��x�̏����l")] private float _patrollingSpeedBase;
        [SerializeField][Tooltip("���G���̑��x")] private float _searchSpeedBase;
        [SerializeField][Tooltip("�ǐՎ��̑��x")] private float _chaseSpeedBase;
        [SerializeField][Tooltip("���͂̏����l�B0�͑S����������100�͂ǂ�ȏ������������������Ȃ�")][Range(0, 100)] private float _audiometerPowerBase;
        [SerializeField][Tooltip("���ɔ������邩�ǂ����̏����l")] private bool _reactToLightBase;
        [SerializeField][Tooltip("��s���Ă��邩�ǂ����̏����l")] private bool _flyingBase;
        [SerializeField][Tooltip("�X�^�~�i�̏����l")] private int _staminaBase;
        [SerializeField][Tooltip("����s���̃N�[���^�C��")] private int _actionCoolTimeBase;
        [SerializeField][Tooltip("������State")] private EnemyState _enemyStateBase;
        [SerializeField][Tooltip("�����̏����l")][Range(0, 1.0f)] private float _footSoundBase;

        [Header("�G�L�����̍U�����\�̏����l")]
        [SerializeField][Tooltip("�U���͂̏����l")] private int _atackPowerBase;
        [SerializeField][Tooltip("San�ւ̍U����")] private int _horrorBase;
        /*�g�p���ɏ����Ă��Ȃ����ǒǉ������ϐ��R�c�B����EnemyAtack�ɂ��邯�ǁA���ꂠ���Ă������傤�Ԃ����Ȃ炱���ɂ˂������UniRx�ɑΉ�������
         * 
        [SerializeField][Tooltip("�U���̃��[�g�̏����l")] private float _atackrateBase;
        [SerializeField][Tooltip("���u�U���\�ł��邩�ǂ���")] private bool _canShot;
        [SerializeField][Tooltip("���u�U���̏����l")] private int _ShotPower;
        [SerializeField][Tooltip("���u�U���̃��[�g�̏����l")]private float _shotRateBase;
        [SerializeField][Tooltip("���u�U���̎˒��̏����l")] private float _shotRateBase;
        */

        [Header("���̑�")]
        [SerializeField][Tooltip("�P�ގ��ɗ��Ƃ����j�[�N�A�C�e��")] private GameObject _uniqueItem;
        [SerializeField][Tooltip("�G�̊o���Ă���\���̂������")] private List<EnemyMagic> _enemyMagics;
        [SerializeField][Tooltip("��̂̊o���Ă������")] private int _hasMagicNum;
        [SerializeField][Tooltip("�ގU���畜�A����܂ł̃~���b")] private int _fallBackTime;
        [SerializeField][Tooltip("�����ڂ̃Q�[���I�u�W�F�N�g")] private GameObject _visual;

        [Header("���g�ɂ��Ă���ł��낤�X�N���v�g")]
        [SerializeField] EnemySearch _enemySearch;
        [SerializeField] EnemyAttack _enemyAttack;
        [SerializeField] EnemyMove _enemyMove;
        [SerializeField] EnemyUniqueAction _enemyUniqueAction;

        private AudioSource _audioSource;



        [Header("�f�o�b�O���邩�ǂ���")]
        [SerializeField] private bool _debugMode;


        private IntReactiveProperty _hp = new IntReactiveProperty();
        private FloatReactiveProperty _patrollingSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _searcSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _chaseSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _audiometerPower = new FloatReactiveProperty();
        private BoolReactiveProperty _reactToLight = new BoolReactiveProperty();
        private BoolReactiveProperty _flying = new BoolReactiveProperty();
        private IntReactiveProperty _stamina = new IntReactiveProperty();
        private IntReactiveProperty _actionCoolTime = new IntReactiveProperty();
        private ReactiveProperty<EnemyState> _enemyState = new ReactiveProperty<EnemyState>(EnemyState.Patrolling);

        private IntReactiveProperty _horror = new IntReactiveProperty();
        private IntReactiveProperty _atackPower = new IntReactiveProperty();

        private BoolReactiveProperty _isBind = new BoolReactiveProperty(false);//�S����Ԃł��邩�ۂ�


        private bool _isCheckWaterEffect = false;//���̐���������Ă��邩�ۂ�
        private bool _isWaterEffectDebuff = false;//���̐���������Ă��邩�ۂ�

        public IObservable<int> OnHpChange { get { return _hp; } }
        public IObservable<float> OnPatrollingSpeedChange { get { return _patrollingSpeed; } }
        public IObservable<float> OnSearchSpeedChange { get { return _searcSpeed; } }
        public IObservable<float> OnChaseSpeedChange { get { return _chaseSpeed; } }
        public IObservable<float> OnAudiometerPowerChange { get { return _audiometerPower; } }
        public IObservable<bool> OnReactToLightChange { get { return _reactToLight; } }
        public IObservable<bool> OnFlyingChange { get { return _flying; } }
        public IObservable<int> OnStaminaChange { get { return _stamina; } }
        public IObservable<EnemyState> OnEnemyStateChange { get { return _enemyState; } }

        public IObservable<int> OnHorrorChange { get { return _horror; } }
        public IObservable<int> OnAtackPowerChange { get { return _atackPower; } }

        public IObservable<bool> OnBindChange { get { return _isBind; } }


        //##########Get�Ƃ�Set�̂����܂�
        public float ReturnPatrollingSpeed { get { return _patrollingSpeed.Value; } }
        public float ReturnSearchSpeed { get { return _searcSpeed.Value; } }
        public float ReturnChaseSpeed { get { return _chaseSpeed.Value; } }

        public int ReturnStaminaBase { get { return _staminaBase; } }
        public int Stamina { get { return _stamina.Value; } }

        public float ReturnAudiomaterPower { get { return _audiometerPower.Value; } }
        public bool ReturnReactToLight { get { return _reactToLight.Value; } }
        public EnemyState ReturnEnemyState { get { return _enemyState.Value; } }

        public int ReturnHorror { get { return _horror.Value; } }
        public int ReturnAtackPower { get { return _atackPower.Value; } }
        public bool ReturnBind { get { return _isBind.Value; } }
        public bool ReturnWaterEffectDebuff { get { return _isWaterEffectDebuff; } }



        private NavMeshAgent _myAgent;


        //##########UniRx�ɂ������Ȃ��ϐ�
        private EnemyVisibilityMap _myEnemyVisivilityMap;

        /// <summary>
        /// �����ݒ������B�O������Ăяo�����ƂƂ���
        /// </summary>
        /// <param name="visivilityMap">����Enemy�̈���EnemyVisivilityMap</param>
        /// <returns>����ɃZ�b�g�A�b�v�ł������ǂ���</returns>
        public bool Init(EnemyVisibilityMap visivilityMap) {
            //�����l��ݒ肵�Ă䂭
            ResetStatus();
            

            //���g�ɂ��Ă��郁�\�b�h�̏�����
            _enemySearch.Init(visivilityMap);
            _enemyAttack.Init(visivilityMap.DeepCopy());//Atack�̓T�[�`�̌��Init
            _enemyMove.Init();
            _enemyUniqueAction.Init(_actionCoolTime.Value);

            //���j���ꂽ���Ƃ����o
            OnHpChange.Where(hp => hp <= 0).Subscribe(hp =>
            {
                FallBack();
            }).AddTo(this);


            //�S����ԂɂȂ����u�ԁE�������u�Ԃɑ��x��ύX
            _myAgent = GetComponent<NavMeshAgent>();
            OnBindChange
                .Skip(1)//�������̎��͖���
                .Subscribe(x =>
                {
                    if (x) { _searcSpeed.Value = _searchSpeedBase * 0.1f; _patrollingSpeed.Value = _patrollingSpeedBase * 0.1f; _chaseSpeed.Value = _chaseSpeedBase * 0.1f; }
                    else { _searcSpeed.Value = _searchSpeedBase * 1f; _patrollingSpeed.Value = _patrollingSpeedBase * 1f; _chaseSpeed.Value = _chaseSpeedBase * 1f; }
                    switch (ReturnEnemyState)
                    {
                        case EnemyState.Patrolling: _myAgent.speed = ReturnPatrollingSpeed; break;
                        case EnemyState.Searching: _myAgent.speed = ReturnSearchSpeed; break;
                        case EnemyState.Chase: _myAgent.speed = ReturnChaseSpeed; break;
                        case EnemyState.Attack: _myAgent.speed = ReturnChaseSpeed; break;
                    }
                }).AddTo(this);

            this.gameObject.TryGetComponent<AudioSource>(out _audioSource);
            _audioSource.volume = _footSoundBase;

            return true;
        }

        /*
         if (x)//�S����ԂɂȂ����u��
                        _myAgent.speed *= 0.1f;
                    else//�S����Ԃ��������u��
                        _myAgent.speed *= 10;
         */

        // Update is called once per frame
        void Update()
        {
            if (_debugMode && Input.GetKey(KeyCode.Z)) { FallBack(); }

            //���̉e���Ŏ����̑��x��������̂�, �������傫���Ȃ�̂����m�F
            if (_isCheckWaterEffect)
            {
                if (_flying.Value)//���ď�Ԃ̎��͉e�����󂯂Ȃ�
                {
                    _isWaterEffectDebuff = false;
                }
                else //���ď�ԂłȂ����͉e�����󂯂�
                {
                    _isWaterEffectDebuff = true;                   
                }

                //�����̑傫����ύX
                _audioSource.volume = _footSoundBase * (_isWaterEffectDebuff ? 1.5f : 1);
            }
        }

        public void SetEnemyState(EnemyState state) {
            if (_debugMode) { Debug.Log("State�ύX" + _enemyState.Value); }
            _enemyState.Value = state;
        }

        /// <summary>
        /// �X�e�[�^�X�̂ݏ���������
        /// </summary>
        public void ResetStatus() {
            _hp.Value = _hpBase;
            _patrollingSpeed.Value = _patrollingSpeedBase;
            _searcSpeed.Value = _searchSpeedBase;
            _searcSpeed.Value = _searchSpeedBase;
            _chaseSpeed.Value = _chaseSpeedBase;
            _atackPower.Value = _atackPowerBase;
            _audiometerPower.Value = _audiometerPowerBase;
            _reactToLight.Value = _reactToLightBase;
            _flying.Value = _flyingBase;
            _stamina.Value = _staminaBase;
            _actionCoolTime.Value = _actionCoolTimeBase;
            _enemyState.Value = _enemyStateBase;

            _horror.Value = _horrorBase;
        }

        /// <summary>
        /// �U���������邽�߂Ɏg�p����
        /// </summary>
        /// <param name="damage">�^����_���[�W</param>
        public void AddDamage(int damage) {
            _hp.Value -= damage;
        }


        /// <summary>
        /// �ގU�����邽�߂Ɏg�p����
        /// </summary>
        public void FallBack() { 
            //�@�\���~
            _enemyAttack.enabled = false;
            _enemyMove.enabled = false; 
            _enemySearch.enabled = false;
            _visual.active = false;
            GameObject.Instantiate(_uniqueItem,this.transform.position,Quaternion.identity);
            Debug.Log(this.gameObject.name + "�ގU���܂����I");
            ReMap(this.GetCancellationTokenOnDestroy()).Forget();
        }

        /// <summary>
        /// �X�^�~�i�̒l������������̂Ɏg�p����
        /// </summary>
        /// <param name="changeStamina">����������X�^�~�i�̒l</param>
        public void StaminaChange(int changeStamina) { 
            _stamina.Value = changeStamina;
        }

        private async Cysharp.Threading.Tasks.UniTaskVoid ReMap(CancellationToken ct)
        {
            await Task.Delay(_fallBackTime,ct);
            //�@�\���~
            _enemyAttack.enabled = true;
            _enemyMove.enabled = true;
            _enemySearch.enabled = true;
            _visual.active = true;
        }

        public void ChangeBindBool(bool value)
        {
            _isBind.Value = value;
        }

        /// <summary>
        /// �u���̐����v�����̌��ʂ��󂯂邩�ۂ������肷��֐�
        /// </summary>
        public void ChangeCheckWaterEffectBool(bool value)
        { 
            _isCheckWaterEffect = value;
            if (_isCheckWaterEffect)
            {
                if (_flying.Value)//���ď�Ԃ̎��͉e�����󂯂Ȃ�
                {
                    _isWaterEffectDebuff = false;
                }
                else //���ď�ԂłȂ����͉e�����󂯂�
                {
                    _isWaterEffectDebuff = true;
                }
            }
            else //���̐������I������Ƃ��ɁA�e�ϐ��������l�ɖ߂�
            {
                _isWaterEffectDebuff = false; 
                _audioSource.volume = _footSoundBase;
            }
        }
    }
}