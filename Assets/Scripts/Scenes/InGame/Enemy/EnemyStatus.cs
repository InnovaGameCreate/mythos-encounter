using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵のスペックと現在の状態を記録する
    /// </summary>
    public class EnemyStatus : MonoBehaviour
    {

        [Header("敵キャラのスペックの初期値")]
        [SerializeField][Tooltip("hpの初期値")] private int _hpBase;
        [SerializeField][Tooltip("巡回時の速度の初期値")] private float _patolloringSpeedBase;
        [SerializeField][Tooltip("索敵時の速度")] private float _searchSpeedBase;
        [SerializeField][Tooltip("追跡時の速度")] private float _chaseSpeedBase;
        [SerializeField][Tooltip("攻撃力の初期値")] private int _atackPowerBase;
        [SerializeField][Tooltip("聴力の初期値")] private float _audiometerPowerBase;
        [SerializeField][Tooltip("光に反応するかどうかの初期値")] private bool _reactToLightBase;
        [SerializeField][Tooltip("飛行しているかどうかの初期値")] private bool _flyingBase;
        [SerializeField][Tooltip("スタミナの初期値")] private int _staminaBase;
        [SerializeField][Tooltip("特殊行動のクールタイム")] private int _actionCoolTime;
        [SerializeField][Tooltip("敵の覚えている可能性のある呪文")] private List<EnemyMagic> _enemyMagics;
        [SerializeField][Tooltip("一個体の覚えている呪文")] private int _hasMagicNum;
        [SerializeField][Tooltip("初期のState")]private EnemyState _enemyStateBase;

        
        private IntReactiveProperty _hp = new IntReactiveProperty();
        private FloatReactiveProperty _patolloringSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _searcSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _cheseSpeed = new FloatReactiveProperty();
        private IntReactiveProperty _atackPower = new IntReactiveProperty();
        private FloatReactiveProperty _audiometerPower = new FloatReactiveProperty();
        private BoolReactiveProperty _reactToLight = new BoolReactiveProperty();
        private BoolReactiveProperty _flying = new BoolReactiveProperty();
        private IntReactiveProperty _stamina = new IntReactiveProperty();
        private BoolReactiveProperty _actionCool = new BoolReactiveProperty();
        /// <summary>
        /// Enum(enemyState)をintとしてリアクティブプロパティにしています。castして使えば大丈夫
        /// </summary>
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


        /// <summary>
        /// 初期設定をする。外部から呼び出すこととする
        /// </summary>
        /// <returns>正常にセットアップできたかどうか</returns>
        public bool Init() {
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
            _enemyState.Value = _enemyStateBase;
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
    }
}