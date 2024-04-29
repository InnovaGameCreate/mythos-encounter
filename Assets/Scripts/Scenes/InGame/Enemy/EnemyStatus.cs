using Scenes.Ingame.Player;
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
        //Countはタイミングを計るような変数を表す。例えば..クールダウンがどれだけ終了しているかや何かい攻撃をしたかなど
        [Header("敵キャラの基本スペックの初期値")]
        [SerializeField][Tooltip("hpの初期値")] private int _hpBase;
        [SerializeField][Tooltip("巡回時の速度の初期値")] private float _patolloringSpeedBase;
        [SerializeField][Tooltip("索敵時の速度")] private float _searchSpeedBase;
        [SerializeField][Tooltip("追跡時の速度")] private float _chaseSpeedBase;       
        [SerializeField][Tooltip("聴力の初期値。0は全く聞こえず100はどんな小さい音も聞き逃さない")][Range(0, 100)] private float _audiometerPowerBase;
        [SerializeField][Tooltip("光に反応するかどうかの初期値")] private bool _reactToLightBase;
        [SerializeField][Tooltip("飛行しているかどうかの初期値")] private bool _flyingBase;
        [SerializeField][Tooltip("スタミナの初期値")] private int _staminaBase;
        [SerializeField][Tooltip("特殊行動のクールタイム")] private int _actionCoolTimeBase;
        [SerializeField][Tooltip("敵の覚えている可能性のある呪文")] private List<EnemyMagic> _enemyMagics;
        [SerializeField][Tooltip("一個体の覚えている呪文")] private int _hasMagicNum;
        [SerializeField][Tooltip("初期のState")] private EnemyState _enemyStateBase;

        [Header("敵キャラの攻撃性能の初期値")]
        [SerializeField][Tooltip("Sanへの攻撃力")] private int _horrorBase;
        [SerializeField][Tooltip("攻撃力の初期値")] private int _atackPowerBase;
        /*使用書に書いていないけど追加した変数軍団。今はEnemyAtackにあるけど、これあってだいじょうぶそうならここにねじこんでUniRxに対応させる
        [SerializeField][Tooltip("攻撃のレートの初期値")] private float _atackrateBase;
        [SerializeField][Tooltip("遠隔攻撃可能であるかどうか")] private bool _canShot;
        [SerializeField][Tooltip("遠隔攻撃の初期値")] private int _ShotPower;
        [SerializeField][Tooltip("遠隔攻撃のレートの初期値")]private float _shotRateBase;
        [SerializeField][Tooltip("遠隔攻撃の射程の初期値")] private float _shotRateBase;
        */


        [Header("自身についているであろうスクリプト")]
        [SerializeField] EnemySearch _enemySearch;
        [SerializeField] EnemyAttack _enemyAttack;
        [SerializeField] EnemyMove _enemyMove;

        private IntReactiveProperty _hp = new IntReactiveProperty();
        private FloatReactiveProperty _patolloringSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _searcSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _cheseSpeed = new FloatReactiveProperty();
        private FloatReactiveProperty _audiometerPower = new FloatReactiveProperty();
        private BoolReactiveProperty _reactToLight = new BoolReactiveProperty();
        private BoolReactiveProperty _flying = new BoolReactiveProperty();
        private IntReactiveProperty _stamina = new IntReactiveProperty();
        private FloatReactiveProperty _actionCoolTime = new FloatReactiveProperty();
        private ReactiveProperty<EnemyState>  _enemyState = new ReactiveProperty<EnemyState>(EnemyState.None);

        private IntReactiveProperty _horror = new IntReactiveProperty();
        private IntReactiveProperty _atackPower = new IntReactiveProperty();


        public IObservable<int> OnHpChange { get { return _hp; }  }
        public IObservable<float> OnPatolloringSppedChange { get { return _patolloringSpeed; } }
        public IObservable<float> OnSearchSpeedChange { get { return _searcSpeed; } }
        public IObservable<float> OnCheseSpeedChange { get { return _cheseSpeed;} }
        public IObservable<float> OnAudiometerPowerChange { get { return _audiometerPower; } }
        public IObservable<bool> OnReactToLightChange { get { return _reactToLight; } }
        public IObservable<bool> OnFlyingChange { get { return _flying; } }
        public IObservable<int> OnStaminaChange { get { return _stamina; } }
        public IObservable<EnemyState> OnEnemyStateChange { get { return _enemyState; } }

        public IObservable<int> OnHorrorChange { get { return _horror; } }
        public IObservable<int> OnAtackPowerChange { get { return _atackPower; } }


        
        public float ReturnAudiomaterPower { get { return _audiometerPower.Value; } }
        public bool ReturnReactToLight { get { return _reactToLight.Value; } }
        public EnemyState ReturnEnemyState { get { return _enemyState.Value; } }

        public int ReturnHorror { get { return _horror.Value; } }
        public int ReturnAtackPower { get { return _atackPower.Value; } }

        //##########UniRxにかかわらない変数
        private EnemyVisibilityMap _myEnemyVisivilityMap;

        /// <summary>
        /// 初期設定をする。外部から呼び出すこととする
        /// </summary>
        /// <param name="visivilityMap">このEnemyの扱うEnemyVisivilityMap</param>
        /// <returns>正常にセットアップできたかどうか</returns>
        public bool Init(EnemyVisibilityMap visivilityMap) {
            //初期値を設定してゆく
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
            

            //自身についているメソッドの初期値を設定してゆく
            _enemySearch.Init(visivilityMap);
            _enemyAttack.Init(visivilityMap.DeepCopy());//Atackはサーチの後にInit
            _enemyMove.Init();


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

        public void SetEnemyState(EnemyState state) {
            _enemyState.Value = state;
        }

    }
}