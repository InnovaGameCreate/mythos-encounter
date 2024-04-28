using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// �G�̃X�y�b�N�ƌ��݂̏�Ԃ��L�^����
    /// </summary>
    public class EnemyStatus : MonoBehaviour
    {

        [Header("�G�L�����̃X�y�b�N�̏����l")]
        [SerializeField][Tooltip("hp�̏����l")] private int _hpBase;
        [SerializeField][Tooltip("���񎞂̑��x�̏����l")] private float _patolloringSpeedBase;
        [SerializeField][Tooltip("���G���̑��x")] private float _searchSpeedBase;
        [SerializeField][Tooltip("�ǐՎ��̑��x")] private float _chaseSpeedBase;
        [SerializeField][Tooltip("�U���͂̏����l")] private int _atackPowerBase;
        [SerializeField][Tooltip("���͂̏����l�B0�͑S����������100�͂ǂ�ȏ������������������Ȃ�")][Range(0, 100)] private float _audiometerPowerBase;
        [SerializeField][Tooltip("���ɔ������邩�ǂ����̏����l")] private bool _reactToLightBase;
        [SerializeField][Tooltip("��s���Ă��邩�ǂ����̏����l")] private bool _flyingBase;
        [SerializeField][Tooltip("�X�^�~�i�̏����l")] private int _staminaBase;
        [SerializeField][Tooltip("����s���̃N�[���^�C��")] private int _actionCoolTimeBase;
        [SerializeField][Tooltip("�G�̊o���Ă���\���̂������")] private List<EnemyMagic> _enemyMagics;
        [SerializeField][Tooltip("��̂̊o���Ă������")] private int _hasMagicNum;
        [SerializeField][Tooltip("������State")] private EnemyState _enemyStateBase;

        [Header("���g�ɂ��Ă���ł��낤�X�N���v�g")]
        [SerializeField] EnemySearch _enemySearch;


        private IntReactiveProperty _hp = new IntReactiveProperty();
        private FloatReactiveProperty _patolloringSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _searcSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _cheseSpeed = new FloatReactiveProperty();
        private IntReactiveProperty _atackPower = new IntReactiveProperty();
        private FloatReactiveProperty _audiometerPower = new FloatReactiveProperty();
        private BoolReactiveProperty _reactToLight = new BoolReactiveProperty();
        private BoolReactiveProperty _flying = new BoolReactiveProperty();
        private IntReactiveProperty _stamina = new IntReactiveProperty();
        private FloatReactiveProperty _actionCoolTime = new FloatReactiveProperty();
        private ReactiveProperty<EnemyState>  _enemyState = new ReactiveProperty<EnemyState>();


        public IObservable<int> OnHpChange { get { return _hp; }  }
        public IObservable<float> OnPatolloringSppedChange { get { return _patolloringSpeed; } }
        public IObservable<float> OnSearchSpeedChange { get { return _searcSpeed; } }
        public IObservable<float> OnCheseSpeedChange { get { return _cheseSpeed;} }
        public IObservable<int> OnAtackPowerChange { get { return _atackPower; } }
        public IObservable<float> OnAudiometerPowerChange { get { return _audiometerPower; } }
        public IObservable<bool> OnReactToLightChange { get { return _reactToLight; } }
        public IObservable<bool> OnFlyingChange { get { return _flying; } }
        public IObservable<int> OnStaminaChange { get { return _stamina; } }
        public IObservable<EnemyState> OnEnemyStateChange { get { return _enemyState; } }


        public float ReturnAudiomaterPower { get { return _audiometerPower.Value; } }
        public bool ReturnReactToLight { get { return _reactToLight.Value; } }
        public EnemyState ReturnEnemyState { get { return _enemyState.Value; } }
        

        /// <summary>
        /// �����ݒ������B�O������Ăяo�����ƂƂ���
        /// </summary>
        /// <returns>����ɃZ�b�g�A�b�v�ł������ǂ���</returns>
        public bool Init() {
            //�����l��ݒ肵�Ă䂭
            _hp.Value = _hpBase;
            _patolloringSpeed.Value = _patolloringSpeedBase;
            _searcSpeed.Value = _searchSpeedBase;
            _searcSpeed.Value = _searchSpeedBase;
            _cheseSpeed.Value = _chaseSpeedBase;
            _atackPower.Value = _atackPowerBase;
            _audiometerPower.Value = _audiometerPowerBase;
            _reactToLight.Value = _reactToLightBase;
            _flying.Value = _flyingBase;
            _stamina.Value = _staminaBase;
            _actionCoolTime.Value = _actionCoolTimeBase;
            _enemyState.Value = _enemyStateBase;
            

            //���g�ɂ��Ă��郁�\�b�h�̏����l��ݒ肵�Ă䂭
            _enemySearch.Init();


            return true;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ChangeEnemyState(EnemyState state) {
            _enemyState.Value = state;
        }

    }
}