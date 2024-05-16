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
        [SerializeField][Tooltip("���񎞂̑��x�̏����l")] private float _patolloringSpeedBase;
        [SerializeField][Tooltip("���G���̑��x")] private float _searchSpeedBase;
        [SerializeField][Tooltip("�ǐՎ��̑��x")] private float _chaseSpeedBase;
        [SerializeField][Tooltip("���͂̏����l�B0�͑S����������100�͂ǂ�ȏ������������������Ȃ�")][Range(0, 100)] private float _audiometerPowerBase;
        [SerializeField][Tooltip("���ɔ������邩�ǂ����̏����l")] private bool _reactToLightBase;
        [SerializeField][Tooltip("��s���Ă��邩�ǂ����̏����l")] private bool _flyingBase;
        [SerializeField][Tooltip("�X�^�~�i�̏����l")] private int _staminaBase;
        [SerializeField][Tooltip("����s���̃N�[���^�C��")] private int _actionCoolTimeBase;
        [SerializeField][Tooltip("������State")] private EnemyState _enemyStateBase;

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



        [Header("�f�o�b�O���邩�ǂ���")]
        [SerializeField] private bool _debugMode;




        private IntReactiveProperty _hp = new IntReactiveProperty();
        private FloatReactiveProperty _patolloringSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _searcSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _cheseSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _audiometerPower = new FloatReactiveProperty();
        private BoolReactiveProperty _reactToLight = new BoolReactiveProperty();
        private BoolReactiveProperty _flying = new BoolReactiveProperty();
        private IntReactiveProperty _stamina = new IntReactiveProperty();
        private FloatReactiveProperty _actionCoolTime = new FloatReactiveProperty();
        private ReactiveProperty<EnemyState> _enemyState = new ReactiveProperty<EnemyState>(EnemyState.Patorolling);

        private IntReactiveProperty _horror = new IntReactiveProperty();
        private IntReactiveProperty _atackPower = new IntReactiveProperty();


        public IObservable<int> OnHpChange { get { return _hp; } }
        public IObservable<float> OnPatolloringSppedChange { get { return _patolloringSpeed; } }
        public IObservable<float> OnSearchSpeedChange { get { return _searcSpeed; } }
        public IObservable<float> OnCheseSpeedChange { get { return _cheseSpeed; } }
        public IObservable<float> OnAudiometerPowerChange { get { return _audiometerPower; } }
        public IObservable<bool> OnReactToLightChange { get { return _reactToLight; } }
        public IObservable<bool> OnFlyingChange { get { return _flying; } }
        public IObservable<int> OnStaminaChange { get { return _stamina; } }
        public IObservable<EnemyState> OnEnemyStateChange { get { return _enemyState; } }

        public IObservable<int> OnHorrorChange { get { return _horror; } }
        public IObservable<int> OnAtackPowerChange { get { return _atackPower; } }


        //##########Get�Ƃ�Set�̂����܂�
        public float ReturnPatolloringSpeed { get { return _patolloringSpeed.Value; } }
        public float ReturnSearchSpeed { get { return _searcSpeed.Value; } }
        public float ReturnCheseSpeed { get { return _cheseSpeed.Value; } }

        public int ReturnStaminaBase { get { return _staminaBase; } }
        public int Stamina { get { return _stamina.Value; } }

        public float ReturnAudiomaterPower { get { return _audiometerPower.Value; } }
        public bool ReturnReactToLight { get { return _reactToLight.Value; } }
        public EnemyState ReturnEnemyState { get { return _enemyState.Value; } }

        public int ReturnHorror { get { return _horror.Value; } }
        public int ReturnAtackPower { get { return _atackPower.Value; } }
        
        




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

            //���j���ꂽ���Ƃ����o
            OnHpChange.Where(hp => hp <= 0).Subscribe(hp =>
            {
                FallBack();
            }).AddTo(this);
            return true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_debugMode && Input.GetKey(KeyCode.Z)) { FallBack(); }
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

    }
}