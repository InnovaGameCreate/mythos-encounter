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

        [Header("自身についているであろうスクリプト")]
        [SerializeField] EnemySearch _enemySearch;
        [SerializeField] EnemyAttack _enemyAttack;
        [SerializeField] EnemyMove _enemyMove;
        [SerializeField] EnemyUniqueAction _enemyUniqueAction;




        [Header("デバッグするかどうか")]
        [SerializeField] private bool _debugMode;

        [Tooltip("現在の敵の速度です")]private FloatReactiveProperty _speed = new FloatReactiveProperty();
        private IntReactiveProperty _hp = new IntReactiveProperty();
        private FloatReactiveProperty _audiometerPower = new FloatReactiveProperty();
        private IntReactiveProperty _stamina = new IntReactiveProperty();
        private ReactiveProperty<EnemyState> _enemyState = new ReactiveProperty<EnemyState>(EnemyState.Patrolling);

        private BoolReactiveProperty _isBind = new BoolReactiveProperty(false);//拘束状態であるか否か
        private FloatReactiveProperty _stiffnessTime = new FloatReactiveProperty(0);//硬直時間


        private bool _isCheckWaterEffect = false;//水の生成がされているか否か
        private BoolReactiveProperty _isWaterEffectDebuff = new BoolReactiveProperty(false);//水の生成によるデバフがされているか否か
        [Tooltip("スタミナが切れて走れない状態か否か")]private BoolReactiveProperty _staminaOver = new BoolReactiveProperty(false);


        public IObservable<int> OnHpChange { get { return _hp; } }

        public IObservable<float> OnAudiometerPowerChange { get { return _audiometerPower; } }

        public IObservable<int> OnStaminaChange { get { return _stamina; } }
        public IObservable<EnemyState> OnEnemyStateChange { get { return _enemyState; } }

        public IObservable<bool> OnBindChange { get { return _isBind; } }

        public IObservable<float> OnStiffnessTimeChange { get { return _stiffnessTime; } }

        public IObservable<bool> OnIsWaterEffectDebuffChange { get { return _isWaterEffectDebuff; } }

        public IObservable<float> OnSpeedChange { get { return _speed; } }

        public IObservable<bool> OnStaminaOverChange { get { return _staminaOver; } }

        //##########GetとかSetのかたまり
        public float ReturnPatrollingSpeed { get { return _patrollingSpeed; } }
        public float ReturnSearchSpeed { get { return _searchSpeed; } }
        public float ReturnChaseSpeed { get { return _chaseSpeed; } }

        public int ReturnStaminaBase { get { return _staminaBase; } }
        public int Stamina { get { return _stamina.Value; } }

        public float ReturnAudiomaterPower { get { return _audiometerPower.Value; } }
        public bool ReturnReactToLight { get { return _reactToLight; } }
        public EnemyState ReturnEnemyState { get { return _enemyState.Value; } }

        public int ReturnHorror { get { return _horror; } }

        public float GetStiffnessTime { get { return _stiffnessTime.Value; } }

        public float GetSpeed { get { return _speed.Value; }}

        public float GetBrindCheseTime { get { return _blindCheseTime; } }

        public bool GetStaminaOver { get { return _staminaOver.Value; } }

        public bool GetIsBind { get { return _isBind.Value; } }

        public bool GetWaterEffectDebuff { get { return _isWaterEffectDebuff.Value; } }





        //##########UniRxにかかわらない変数
        private EnemyVisibilityMap _myEnemyVisivilityMap;
        private AudioSource _audioSource;

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
            _enemyUniqueAction.Init(_actionCoolTime);

            //撃破されたことを検出
            OnHpChange.Where(hp => hp <= 0).Subscribe(hp =>
            {
                FallBack();
            }).AddTo(this);

            ////////////


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
                if (_flying)//飛翔状態の時は影響を受けない
                {
                    _isWaterEffectDebuff.Value = false;
                }
                else //飛翔状態でない時は影響を受ける
                {
                    _isWaterEffectDebuff.Value = true;                   
                }

                //足音の大きさを変更
                _audioSource.volume = _footSoundBase * (_isWaterEffectDebuff.Value ? 1.5f : 1);
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
            _audiometerPower.Value = _audiometerPowerBase;
            _stamina.Value = _staminaBase;
            _enemyState.Value = _enemyStateBase;
        }

        /// <summary>
        /// 攻撃を加えるために使用する
        /// </summary>
        /// <param name="damage">与えるダメージ</param>
        public void AddDamage(int damage) {
            _hp.Value -= damage;
        }

        /// <summary>
        /// 移動速度を指定した値に設定する
        /// </summary>
        /// <param name="value">設定する速度</param>
        public void SetSpeed(float value) { { _speed.Value = value; } }

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

        public void SetStuminaOver(bool setValue) { 
            _staminaOver.Value = setValue;
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
                if (_flying)//飛翔状態の時は影響を受けない
                {
                    _isWaterEffectDebuff.Value = false;
                }
                else //飛翔状態でない時は影響を受ける
                {
                    _isWaterEffectDebuff.Value = true;
                }
            }
            else //水の生成が終わったときに、各変数を初期値に戻す
            {
                _isWaterEffectDebuff.Value = false; 
                _audioSource.volume = _footSoundBase;
            }
        }
    }
}