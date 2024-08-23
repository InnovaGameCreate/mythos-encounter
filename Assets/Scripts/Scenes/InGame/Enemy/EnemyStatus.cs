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
    /// 敵のスペックと現在の状態を記録する
    /// </summary>
    public class EnemyStatus : MonoBehaviour
    {
        //Countはタイミングを計るような変数を表す。例えば..クールダウンがどれだけ終了しているかや何かい攻撃をしたかなど
        [Header("敵キャラの基本スペックの初期値")]
        [SerializeField][Tooltip("hpの初期値")] private int _hpBase;
        [SerializeField][Tooltip("巡回時の速度の初期値")] private float _patrollingSpeedBase;
        [SerializeField][Tooltip("索敵時の速度")] private float _searchSpeedBase;
        [SerializeField][Tooltip("追跡時の速度")] private float _chaseSpeedBase;
        [SerializeField][Tooltip("聴力の初期値。0は全く聞こえず100はどんな小さい音も聞き逃さない")][Range(0, 100)] private float _audiometerPowerBase;
        [SerializeField][Tooltip("光に反応するかどうかの初期値")] private bool _reactToLightBase;
        [SerializeField][Tooltip("飛行しているかどうかの初期値")] private bool _flyingBase;
        [SerializeField][Tooltip("スタミナの初期値")] private int _staminaBase;
        [SerializeField][Tooltip("特殊行動のクールタイム")] private int _actionCoolTimeBase;
        [SerializeField][Tooltip("初期のState")] private EnemyState _enemyStateBase;
        [SerializeField][Tooltip("足音の初期値")][Range(0, 1.0f)] private float _footSoundBase;

        [Header("敵キャラの攻撃性能の初期値")]
        [SerializeField][Tooltip("Sanへの攻撃力")] private int _horrorBase;

        [Header("その他")]
        [SerializeField][Tooltip("撤退時に落とすユニークアイテム")] private GameObject _uniqueItem;
        [SerializeField][Tooltip("敵の覚えている可能性のある呪文")] private List<EnemyMagic> _enemyMagics;
        [SerializeField][Tooltip("一個体の覚えている呪文")] private int _hasMagicNum;
        [SerializeField][Tooltip("退散から復帰するまでのミリ秒")] private int _fallBackTime;
        [SerializeField][Tooltip("見た目のゲームオブジェクト")] private GameObject _visual;

        [Header("自身についているであろうスクリプト")]
        [SerializeField] EnemySearch _enemySearch;
        [SerializeField] EnemyAttack _enemyAttack;
        [SerializeField] EnemyMove _enemyMove;
        [SerializeField] EnemyUniqueAction _enemyUniqueAction;

        private AudioSource _audioSource;



        [Header("デバッグするかどうか")]
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

        private BoolReactiveProperty _isBind = new BoolReactiveProperty(false);//拘束状態であるか否か
        private FloatReactiveProperty _stiffnessTime = new FloatReactiveProperty(0);//硬直時間


        private bool _isCheckWaterEffect = false;//水の生成がされているか否か
        private bool _isWaterEffectDebuff = false;//水の生成がされているか否か

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

        public IObservable<bool> OnBindChange { get { return _isBind; } }

        public IObservable<float> OnStiffnessTimeChange { get { return _stiffnessTime; } }




        //##########GetとかSetのかたまり
        public float ReturnPatrollingSpeed { get { return _patrollingSpeed.Value; } }
        public float ReturnSearchSpeed { get { return _searcSpeed.Value; } }
        public float ReturnChaseSpeed { get { return _chaseSpeed.Value; } }

        public int ReturnStaminaBase { get { return _staminaBase; } }
        public int Stamina { get { return _stamina.Value; } }

        public float ReturnAudiomaterPower { get { return _audiometerPower.Value; } }
        public bool ReturnReactToLight { get { return _reactToLight.Value; } }
        public EnemyState ReturnEnemyState { get { return _enemyState.Value; } }

        public int ReturnHorror { get { return _horror.Value; } }
        public bool ReturnBind { get { return _isBind.Value; } }
        public bool ReturnWaterEffectDebuff { get { return _isWaterEffectDebuff; } }

        public float GetStiffnessTime { get { return _stiffnessTime.Value; } }



        private NavMeshAgent _myAgent;


        //##########UniRxにかかわらない変数
        private EnemyVisibilityMap _myEnemyVisivilityMap;

        /// <summary>
        /// 初期設定をする。外部から呼び出すこととする
        /// </summary>
        /// <param name="visivilityMap">このEnemyの扱うEnemyVisivilityMap</param>
        /// <returns>正常にセットアップできたかどうか</returns>
        public bool Init(EnemyVisibilityMap visivilityMap) {
            //初期値を設定してゆく
            ResetStatus();
            

            //自身についているメソッドの初期化
            _enemySearch.Init(visivilityMap);
            _enemyAttack.Init(visivilityMap.DeepCopy());//Atackはサーチの後にInit
            _enemyMove.Init();
            _enemyUniqueAction.Init(_actionCoolTime.Value);

            //撃破されたことを検出
            OnHpChange.Where(hp => hp <= 0).Subscribe(hp =>
            {
                FallBack();
            }).AddTo(this);


            //拘束状態になった瞬間・解けた瞬間に速度を変更
            _myAgent = GetComponent<NavMeshAgent>();
            OnBindChange
                .Skip(1)//初期化の時は無視
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
         if (x)//拘束状態になった瞬間
                        _myAgent.speed *= 0.1f;
                    else//拘束状態が解けた瞬間
                        _myAgent.speed *= 10;
         */

        // Update is called once per frame
        void Update()
        {
            if (_debugMode && Input.GetKey(KeyCode.Z)) { FallBack(); }

            if (_stiffnessTime.Value > 0) { 
                _stiffnessTime.Value -= Time.deltaTime;
                if (_stiffnessTime.Value < 0)
                {
                    _stiffnessTime.Value = 0;
                }
            }



            //水の影響で自分の速度が下がるのか, 足音が大きくなるのかを確認
            if (_isCheckWaterEffect)
            {
                if (_flying.Value)//飛翔状態の時は影響を受けない
                {
                    _isWaterEffectDebuff = false;
                }
                else //飛翔状態でない時は影響を受ける
                {
                    _isWaterEffectDebuff = true;                   
                }

                //足音の大きさを変更
                _audioSource.volume = _footSoundBase * (_isWaterEffectDebuff ? 1.5f : 1);
            }

        }

        public void SetEnemyState(EnemyState state) {
            if (_debugMode) { Debug.Log("State変更" + _enemyState.Value); }
            _enemyState.Value = state;
        }

        /// <summary>
        /// ステータスのみ初期化する
        /// </summary>
        public void ResetStatus() {
            _hp.Value = _hpBase;
            _patrollingSpeed.Value = _patrollingSpeedBase;
            _searcSpeed.Value = _searchSpeedBase;
            _searcSpeed.Value = _searchSpeedBase;
            _chaseSpeed.Value = _chaseSpeedBase;
            _audiometerPower.Value = _audiometerPowerBase;
            _reactToLight.Value = _reactToLightBase;
            _flying.Value = _flyingBase;
            _stamina.Value = _staminaBase;
            _actionCoolTime.Value = _actionCoolTimeBase;
            _enemyState.Value = _enemyStateBase;

            _horror.Value = _horrorBase;
        }

        /// <summary>
        /// 攻撃を加えるために使用する
        /// </summary>
        /// <param name="damage">与えるダメージ</param>
        public void AddDamage(int damage) {
            _hp.Value -= damage;
        }


        /// <summary>
        /// 退散させるために使用する
        /// </summary>
        public void FallBack() { 
            //機能を停止
            _enemyAttack.enabled = false;
            _enemyMove.enabled = false; 
            _enemySearch.enabled = false;
            _visual.active = false;
            GameObject.Instantiate(_uniqueItem,this.transform.position,Quaternion.identity);
            Debug.Log(this.gameObject.name + "退散しました！");
            ReMap(this.GetCancellationTokenOnDestroy()).Forget();
        }

        /// <summary>
        /// スタミナの値を書き換えるのに使用する
        /// </summary>
        /// <param name="changeStamina">書き換えるスタミナの値</param>
        public void StaminaChange(int changeStamina) { 
            _stamina.Value = changeStamina;
        }

        private async Cysharp.Threading.Tasks.UniTaskVoid ReMap(CancellationToken ct)
        {
            await Task.Delay(_fallBackTime,ct);
            //機能を停止
            _enemyAttack.enabled = true;
            _enemyMove.enabled = true;
            _enemySearch.enabled = true;
            _visual.active = true;
        }

        public void ChangeBindBool(bool value)
        {
            _isBind.Value = value;
        }


        public void ChangeStiffnessTime(float value) { 
            _stiffnessTime.Value += value;
            }

        /// <summary>
        /// 「水の生成」呪文の効果を受けるか否かを決定する関数
        /// </summary>
        public void ChangeCheckWaterEffectBool(bool value)
        { 
            _isCheckWaterEffect = value;
            if (_isCheckWaterEffect)
            {
                if (_flying.Value)//飛翔状態の時は影響を受けない
                {
                    _isWaterEffectDebuff = false;
                }
                else //飛翔状態でない時は影響を受ける
                {
                    _isWaterEffectDebuff = true;
                }
            }
            else //水の生成が終わったときに、各変数を初期値に戻す
            {
                _isWaterEffectDebuff = false; 
                _audioSource.volume = _footSoundBase;
            }
        }
    }
}