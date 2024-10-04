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
using Fusion;
using UnityEngine.Rendering.HighDefinition;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵のスペックと現在の状態を記録する
    /// </summary>
    public class EnemyStatus : NetworkBehaviour
    {
        //Countはタイミングを計るような変数を表す。例えば..クールダウンがどれだけ終了しているかや何かい攻撃をしたかなど
        [Header("敵キャラの基本スペックの初期値")]
        [SerializeField][Tooltip("hpの初期値")] private int _hpBase;
        [SerializeField][Tooltip("巡回時の速度の初期値")] private float _patrollingSpeed;
        [SerializeField][Tooltip("索敵時の速度")] private float _searchSpeed;
        [SerializeField][Tooltip("追跡時の速度")] private float _chaseSpeed;
        [SerializeField][Tooltip("聴力の初期値。0は全く聞こえず100はどんな小さい音も聞き逃さない")][Range(0, 100)] private float _audiometerPowerBase;
        [SerializeField][Tooltip("光に反応するかどうかの初期値")] private bool _reactToLight;
        [SerializeField][Tooltip("飛行しているかどうかの初期値")] private bool _flying;
        [SerializeField][Tooltip("スタミナの初期値")] private int _staminaBase;
        [SerializeField][Tooltip("特殊行動のクールタイム")] private int _actionCoolTime;
        [SerializeField][Tooltip("初期のState")] private EnemyState _enemyStateBase;
        [SerializeField][Tooltip("足音の初期値")][Range(0, 1.0f)] private float _footSoundBase;
        [SerializeField][Tooltip("プレイヤーを見失った後何秒間はあきらめないか")] private float _blindCheseTime;

        [Header("敵キャラの攻撃性能の初期値")]
        [SerializeField][Tooltip("Sanへの攻撃力")] private int _horror;

        [Header("その他")]
        [SerializeField][Tooltip("撤退時に落とすユニークアイテム")] private GameObject _uniqueItem;
        [SerializeField][Tooltip("敵の覚えている可能性のある呪文")] private List<EnemyMagic> _enemyMagics;
        [SerializeField][Tooltip("一個体の覚えている呪文")] private int _hasMagicNum;
        [SerializeField][Tooltip("退散から復帰するまでのミリ秒")] private int _fallBackTime;
        [SerializeField][Tooltip("見た目のゲームオブジェクト")] private GameObject _visual;
        [SerializeField][Tooltip("発見動作は何ミリ秒間続くのかどうか")] private int _discoverTime;
        [SerializeField][Tooltip("敵キャラクターのID")] private int _enemyId;


        [Header("自身についているであろうスクリプト")]
        [SerializeField] EnemySearch _enemySearch;
        [SerializeField] EnemyAttack _enemyAttack;
        [SerializeField] EnemyMove _enemyMove;
        [SerializeField] EnemyUniqueAction _enemyUniqueAction;




        [Header("デバッグするかどうか")]
        [SerializeField] private bool _debugMode;
        
        private bool _isCheckWaterEffect = false;//水の生成がされているか否か


        //########################
        //[NetWorked]置き場、注意！Fusionの仕様上getsetは独自実装してはならない！
        

        [Networked] public float Speed { get;private set;}
        private Subject<float> _speedSubject = new Subject<float>();
        public IObservable<float> OnSpeedChanged
        {
            get { return _speedSubject; }
        }


        [Networked] public int Hp { get;private set; }
        private Subject<int> _hpSubject = new Subject<int>();
        public IObservable<int> OnHpChanged
        {
            get { return _hpSubject; }
        }

        [Networked] public float AudiometerPower { get; private set; }
        private Subject<float> _audiometerPower = new Subject<float>();
        public IObservable<float> OnAudiometerPowerChange
        {
            get { return _audiometerPower; }
        }

        [Networked] public int Stamina { get; private set; }
        private Subject<int> _staminaSubject = new Subject<int>();
        public IObservable<int> OnStaminaChange
        {
            get { return _staminaSubject; }
        }

        [Networked] public EnemyState State { get; private set; } = EnemyState.Patrolling;
        private Subject<EnemyState> _enemyStateSubject = new Subject<EnemyState>();
        public IObservable<EnemyState> OnEnemyStateChange
        {
            get { return _enemyStateSubject; }
        }

        [Networked] public bool IsBind { get; private set; }
        private Subject<bool> _bindSubject = new Subject<bool>();
        public IObservable<bool> OnBindChange
        {
            get { return _bindSubject; }
        }

        [Networked] public float StiffnessTime { get; private set; }
        private Subject<float> _stiffnessTimeSubject = new Subject<float>();
        public IObservable<float> OnStiffnessTimeChange
        {
            get { return _stiffnessTimeSubject; }
        }

        [Networked] public bool IsWaterEffectDebuff { get; private set; }
        private Subject<bool> _isWaterEffectDebuffSubject = new Subject<bool>();
        public IObservable<bool> OnIsWaterEffectDebuffChange
        {
            get { return _isWaterEffectDebuffSubject; }
        }

        [Networked] public bool StaminaOver { get; private set; }
        private Subject<bool> _staminaOverSubject = new Subject<bool>();
        public IObservable<bool> OnStaminaOverChange
        {
            get { return _staminaOverSubject; }
        }


        //##########GetとかSetのかたまり
        public float PatrollingSpeed { get { return _patrollingSpeed; } }
        public float SearchSpeed { get { return _searchSpeed; } }
        public float ChaseSpeed { get { return _chaseSpeed; } }

        public int StaminaBase { get { return _staminaBase; } }

        public bool ReactToLight { get { return _reactToLight; } }

        public int Horror { get { return _horror; } }


        public float BrindCheseTime { get { return _blindCheseTime; } }



        public int DiscoverTime { get { return _discoverTime; } }
        public int EnemyId { get { return _enemyId; } }





        //##########UniRxにかかわらない変数
        private EnemyVisibilityMap _myEnemyVisivilityMap;
        private AudioSource _audioSource;

        private ChangeDetector _changeDetector;




        /// <summary>
        /// 初期設定をする。外部から呼び出すこととする
        /// </summary>
        /// <param name="visivilityMap">このEnemyの扱うEnemyVisivilityMap</param>
        /// <returns>正常にセットアップできたかどうか</returns>
        public bool Init(EnemyVisibilityMap visivilityMap) {
            //初期値を設定してゆく
            ResetStatus();


            //自身についているメソッドの初期化
            _enemyMove.Init();
            _enemySearch.Init(visivilityMap);
            _enemyAttack.Init(visivilityMap.DeepCopy());//Atackはサーチの後にInit          
            _enemyUniqueAction.Init(_actionCoolTime);

            //撃破されたことを検出
            OnHpChanged.Where(hp => hp <= 0).Subscribe(hp =>
            {
                FallBack();
            }).AddTo(this);

            ////////////
            //変更を検出する準備をする
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

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

        public override void FixedUpdateNetwork()
        {
            //変更を検出しUniRxのイベントを発行す
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(Speed):
                        _speedSubject.OnNext(Speed);
                        break;
                    case nameof(Hp):
                        _hpSubject.OnNext(Hp);
                        break;
                }
            }






            if (_debugMode && Input.GetKey(KeyCode.Z)) { FallBack(); }

            if (StiffnessTime > 0) {
                StiffnessTime -= Time.deltaTime;
                if (StiffnessTime < 0)
                {
                    StiffnessTime = 0;
                }
            }



            //水の影響で自分の速度が下がるのか, 足音が大きくなるのかを確認
            if (_isCheckWaterEffect)
            {
                if (_flying)//飛翔状態の時は影響を受けない
                {
                    IsWaterEffectDebuff = false;
                }
                else //飛翔状態でない時は影響を受ける
                {
                    IsWaterEffectDebuff = true;                   
                }

                //足音の大きさを変更
                _audioSource.volume = _footSoundBase * (IsWaterEffectDebuff ? 1.5f : 1);
            }

        }

        public void SetEnemyState(EnemyState state) {
            if (_debugMode) { Debug.Log("State変更" + State); }
            State = state;
        }

        /// <summary>
        /// ステータスのみ初期化する
        /// </summary>
        public void ResetStatus() {
            Hp = _hpBase;
            AudiometerPower = _audiometerPowerBase;
            Stamina = _staminaBase;
            State = _enemyStateBase;
        }

        /// <summary>
        /// 攻撃を加えるために使用する
        /// </summary>
        /// <param name="damage">与えるダメージ</param>
        public void AddDamage(int damage) {
            Hp -= damage;
        }

        /// <summary>
        /// 移動速度を指定した値に設定する
        /// </summary>
        /// <param name="value">設定する速度</param>
        public void SetSpeed(float value) { { Speed = value; } }

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
            Stamina = changeStamina;
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

        public void SetBind(bool value)
        {
            IsBind = value;
        }

        public void SetStuminaOver(bool setValue) { 
            StaminaOver = setValue;
        }
        /// <summary>
        /// 硬直時間を変化さえる
        /// </summary>
        /// <param name="value">変化する値</param>
        public void ChangeStiffnessTime(float value) { 
            StiffnessTime += value;
        }

        /// <summary>
        /// 水の影響を変更する
        /// </summary>
        /// <param name="value">水の影響があるときはtrue無いときはfalse</param>
        public void SetCheckWaterEffect(bool value)
        { 
            _isCheckWaterEffect = value;
            if (_isCheckWaterEffect)
            {
                if (_flying)//飛翔状態の時は影響を受けない
                {
                    IsWaterEffectDebuff = false;
                }
                else //飛翔状態でない時は影響を受ける
                {
                    IsWaterEffectDebuff = true;
                }
            }
            else //水の生成が終わったときに、各変数を初期値に戻す
            {
                IsWaterEffectDebuff = false; 
                _audioSource.volume = _footSoundBase;
            }
        }
    }
}