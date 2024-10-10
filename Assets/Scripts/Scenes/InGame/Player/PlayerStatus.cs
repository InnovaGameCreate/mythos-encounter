using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Fusion;
using System;
using Common.Player;

/// <summary>
/// プレイヤーのステータスを管理するクラス
/// MV(R)PにおけるModelの役割を想定
/// </summary>
namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 体力, スタミナ,SAN値を変更するときのモードを指定する列挙型 
    /// </summary>
    public enum ChangeValueMode
    { 
        /// <summary>
        /// 回復する
        /// </summary>
        Heal,
        /// <summary>
        /// ダメージを受ける
        /// </summary>
        Damage,
        /// <summary>
        /// ダメージを受ける（体力専用）
        /// </summary>
        Bleeding
    }

    public class PlayerStatus : MonoBehaviour
    {
        //プレイヤーのデータベース(仮置き)
        [Header("プレーヤーのデータベース")]
        [SerializeField] private int _playerID = 0;
        [SerializeField] private int _healthBase = 100;
        [SerializeField] private int _staminaBase = 100;
        [SerializeField] private int _sanBase = 100;
        [SerializeField] private int _speedBase = 5;
        [SerializeField][Tooltip("プレイヤーの持つ照明の光の届く距離")] private float _lightrangeBase = 20;
        [SerializeField][Tooltip("プレイヤーのしゃがみ歩き時の音量")] private float _sneakVolumeBase = 5;
        [SerializeField][Tooltip("プレイヤーの歩き時の音量")] private float _walkVolumeBase = 10;
        [SerializeField][Tooltip("プレイヤーの走りの音量")] private float _runVolumeBase = 15;


        //現在のステータスの変数（今後ネットワーク化予定）
        //初期化についても今後はデータベースを参照して行うようにする。
        [SerializeField] private IntReactiveProperty _health = new IntReactiveProperty();//HP.ゼロになると死亡
        [SerializeField] private IntReactiveProperty _bleedingHealth = new IntReactiveProperty();//出血処理用HP
        [SerializeField] private IntReactiveProperty _stamina = new IntReactiveProperty();//スタミナ
        [SerializeField] private IntReactiveProperty _san = new IntReactiveProperty();//SAN値
        [SerializeField] private IntReactiveProperty _speed = new IntReactiveProperty();//移動速度の基準値
        [SerializeField] private BoolReactiveProperty _survive = new BoolReactiveProperty(true);//生死.trueのときは生きている
        [SerializeField] private BoolReactiveProperty _bleeding = new BoolReactiveProperty(false);//出血状態.trueのときに時間経過で体力が減少

        [SerializeField] private ReactiveProperty<PlayerActionState> _playerActionState = new ReactiveProperty<PlayerActionState>(PlayerActionState.Idle);
        [SerializeField] private FloatReactiveProperty _lightrange = new FloatReactiveProperty();//光の届く距離
        [SerializeField] private FloatReactiveProperty _sneakVolume = new FloatReactiveProperty();//しゃがみ時の音量
        [SerializeField] private FloatReactiveProperty _walkVolume = new FloatReactiveProperty();//しゃがみ時の音量
        [SerializeField] private FloatReactiveProperty _runVolume = new FloatReactiveProperty();//しゃがみ時の音量

        [Header("必要なコンポーネント")]
        [SerializeField] private Animator _anim;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private CapsuleCollider _cupsuleCollider;
        [SerializeField] private PlayerMagic _playerMagic;
        [SerializeField] private PlayerItem _playerItem;
        [SerializeField] private NetworkObject _networkObject;

        private Subject<float> castEvent = new Subject<float>();//呪文の詠唱時間を発行
        private Subject<Unit> cancelCastEvent = new Subject<Unit>();//呪文の詠唱をキャンセルされた際のイベント
        public IObserver<float> OnCastEventCall { get { return castEvent; } }//別のスクリプトから呪文の詠唱時間イベントのOnNextを飛ばせるようにする



        //その他のSubject
        private Subject<Unit> _enemyAttackedMe = new Subject<Unit>();//敵から攻撃を食らったときのイベント
        private Subject<PlayerStatus> playerActionStateChangeEvent = new Subject<PlayerStatus>();//プレイヤーのStateが変化した時のイベント（どのプレイヤーステータスか判別可能）

        //それぞれの購読側を公開する。他のClassでSubscribeできる。
        public IObservable<int> OnPlayerHealthChange { get { return _health; } }//_health(体力)が変化した際にイベントが発行
        public IObservable<int> OnPlayerBleedingHealthChange { get { return _bleedingHealth; } }//_bleedinghealthが変化した際にイベントが発行
        public IObservable<int> OnPlayerStaminaChange { get { return _stamina; } }//_stamina(スタミナ)が変化した際にイベントが発行
        public IObservable<int> OnPlayerSanValueChange { get { return _san; } }//_san(SAN値)が変化した際にイベントが発行
        public IObservable<int> OnPlayerSpeedChange { get { return _speed; } }//_speed(移動速度の基準値)が変化した際にイベントが発行

        public IObservable<bool> OnPlayerSurviveChange { get { return _survive; } }//_survive(生死)が変化した際にイベントが発行
        public IObservable<bool> OnPlayerbleedingChange { get { return _bleeding; } }//_bleeding(出血状態)が変化した際にイベントが発行
        public IObservable<PlayerActionState> OnPlayerActionStateChange { get { return _playerActionState; } }//_PlayerActionState(プレイヤーの行動状態)が変化した際にイベントが発行
        public IObservable<float> OnLightrangeChange { get { return _lightrange; } }//プレイヤーの光の届く距離が変化した場合にイベントが発行
        public IObservable<float> OnSneakVolumeChange { get { return _sneakVolume; } }//プレイヤーの忍び歩きの音が届く距離が変化した場合にイベントが発行
        public IObservable<float> OnWalkVolumeChange { get { return _walkVolume; } }//プレイヤーの歩く音が届く距離が変化した場合にイベントが発行
        public IObservable<float> OnRunVolumeChange { get { return _runVolume; } }//プレイヤーの走る音が届く距離が変化した場合にイベントが発行

        public IObservable<Unit> OnEnemyAttackedMe { get { return _enemyAttackedMe; } }//敵から攻撃を受けた際のイベントを登録させる
        public IObserver<Unit> OnEnemyAttackedMeEvent { get { return _enemyAttackedMe; } }//敵から攻撃を受けた際にイベントが発行

        public IObservable<float> OnCastEvent { get { return castEvent; } }//呪文詠唱を始めた際に呼ばれるイベント
        public IObservable<Unit> OnCancelCastEvent { get { return cancelCastEvent; } }//敵から攻撃を受けた際のイベントを登録させる
        public IObserver<Unit> OnCancelCastEventCall { get { return cancelCastEvent; } }//敵から攻撃を受けた際にイベントが発行

        public IObservable<PlayerStatus> OnPlayerActionStateChangeEvent{get { return playerActionStateChangeEvent; }}//PlayerStateが変化した場合にイベント発行

        //一部情報の開示
        public int playerID { get { return _playerID; } }
        public int health_max { get { return _healthBase; } }
        public int nowPlayerHealth { get { return _health.Value; } }
        public int stamina_max { get { return _staminaBase; } }
        public int nowStaminaValue { get { return _stamina.Value; } }
        public int nowPlayerSanValue { get { return _san.Value; } }
        public int nowPlayerSpeed { get { return _speed.Value; } }

        public bool nowBleedingValue { get { return _bleeding.Value; } }
        public bool nowPlayerSurvive { get { return _survive.Value; } }

        public PlayerActionState nowPlayerActionState { get { return _playerActionState.Value; } }
        public float nowPlayerLightRange { get { return _lightrange.Value; } }
        public float nowPlayerSneakVolume { get { return _sneakVolume.Value; } }
        public float nowPlayerWalkVolume { get { return _walkVolume.Value; } }
        public float nowPlayerRunVolume { get { return _runVolume.Value; } }

        public bool nowPlayerUseMagic { get { return _isUseMagic; } }
        public bool nowReviveAnimationDoing { get { return _startReviveAnimation; } }


        [HideInInspector] public int lastHP;//HPの変動前の数値を記録。比較に用いる
        [HideInInspector] public int lastSanValue;//SAN値の変動前の数値を記録。比較に用いる
        [HideInInspector] public int bleedingDamage = 10;//出血時に受けるダメージ

        private bool _isUseItem = false;
        private bool _isUseMagic = false;
        private bool _isHaveCharm = false;
        private bool _isProtect = false;//バリア呪文でダメージを防ぐか否か
        private bool _isUseEscapePoint = false;
        private bool _isPulsationBleeding = false;

        //アニメーション関連の変数
        private int _deathEventCount = 0;//死亡アニメーションのイベント回数確認用
        private bool _startReviveAnimation = false;//蘇生アニメーションが始まったか否か

        private bool _startDeathAnimation = false;//死亡アニメーションが始まったか否か
        private bool _isBuffedAdrenaline = false;

        private void Init()
        {
            _health.Value = _healthBase;
            _bleedingHealth.Value = _healthBase;
            _stamina.Value = _staminaBase;
            _san.Value = _sanBase;
            _speed.Value = _speedBase;
            _lightrange.Value = _lightrangeBase;
            _sneakVolume.Value = _sneakVolumeBase;
            _walkVolume.Value = _walkVolumeBase;
            _runVolume.Value = _runVolumeBase;

            lastHP = 100;
            lastSanValue = 100;
            bleedingDamage = 10;
        }
        // Start is called before the first frame update
        void Awake()
        {
            //初期化
            Init();
            _health.Subscribe(x => CheckHealth(x, _playerID)).AddTo(this);//体力が変化したときにゲーム内で変更を加える
            _stamina.Subscribe(x => CheckStamina(x, _playerID)).AddTo(this);//スタミナが変化したときにゲーム内で変更を加える
            _san.Subscribe(x => CheckSanValue(x, _playerID)).AddTo(this);//SAN値が変化したときにゲーム内で変更を加える
            _bleeding.
                Where(x => x == true).
                Subscribe(_ => StartCoroutine(Bleeding(bleedingDamage))).AddTo(this);//出血状態になったときに出血処理を開始

            OnEnemyAttackedMe
                .Where(_ => _isUseMagic)
                .Subscribe(x => cancelCastEvent.OnNext(default)).AddTo(this);//攻撃を受けた際に呪文の詠唱時間UIを非表示にする

            _survive
                .Skip(1)
                .Subscribe(x => CheckSurvive(x)).AddTo(this);

            OnPlayerActionStateChange.Skip(1).Subscribe(x => playerActionStateChangeEvent.OnNext(this)).AddTo(this);

            //呪文解放条件関連
            _san
                .Skip(1)
                .First(x => x <= 50)
                //.Where(_ => _networkObject.HasInputAuthority)//入力権限があったら（Fusion対応後にコメントアウト外せ）
                .Subscribe(x => SpellUnlockSystem.Instance.SetSpellUnlockInfoData(3,true)).AddTo(this);//SAN値が初めて50%をきったら自己洗脳呪文の解放条件を満たす

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //デバッグ用.(必要無くなれば消す)
            //デバッグ用.(必要無くなれば消す)
            if (Input.GetKeyDown(KeyCode.L))
            {
                ChangeHealth(10, ChangeValueMode.Damage);
                ChangeSanValue(10, ChangeValueMode.Damage);
                ChangeBleedingBool(true);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                ChangeBleedingBool(false);
                ChangeHealth(10, ChangeValueMode.Heal);
                ChangeSanValue(10, ChangeValueMode.Heal);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                _enemyAttackedMe.OnNext(default);
            }
#endif           

            //死亡時に当たり判定を死体と同じ場所に動かす
            //Todo:蘇生時に当たり判定を体と同じ場所に動かす（今後実装）
            if (_startDeathAnimation || _startReviveAnimation)
            {               
                _cupsuleCollider.height = _anim.GetFloat("ColliderHeight");//　コライダの高さの調整       
                _cupsuleCollider.center = new Vector3(_cupsuleCollider.center.x, _anim.GetFloat("ColliderCenterY"), _cupsuleCollider.center.z);//　コライダの中心位置の調整

                //　コライダの半径の調整
                _cupsuleCollider.radius = _anim.GetFloat("ColliderRadius");

                //CharacterControllerのコライダー半径の変更（死亡時のアバターの壁埋まり防止のため）
                if(_startDeathAnimation && !_startReviveAnimation)//死亡時
                    _controller.radius = 1.2f;
                else if(!_startDeathAnimation && _startReviveAnimation)//蘇生時
                    _controller.radius = 0.4f;


                //　コライダの向きの調整
                if (_anim.GetFloat("ColliderDirection") >= 1.0f)
                {
                    _cupsuleCollider.direction = 2;//Z軸方向の向きに変化
                }
                else
                {
                    _cupsuleCollider.direction = 1;//Y軸方向の向きに変化
                }
            }
        }

        /// <summary>
        /// 体力を変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">値変更のモードを指定</param>
        public void ChangeHealth(int value, ChangeValueMode mode)
        {
            if (mode == ChangeValueMode.Heal)
            {
                lastHP = _health.Value;
                _health.Value = Mathf.Min(100, _health.Value + value);
                _bleedingHealth.Value = Mathf.Min(100, _bleedingHealth.Value + value);

                //if (_networkObject.HasInputAuthority)
                SpellUnlockSystem.Instance.IncreaseHealedHealth(_health.Value - lastHP);
            }
            else if (mode == ChangeValueMode.Damage)
            {
                if (_isProtect)
                {
                    _isProtect = false;
                    Debug.Log("呪文によりダメージが無効化されました。");
                    return;
                }
                lastHP = _health.Value;
                _health.Value = Mathf.Max(0, _health.Value - value);
                _bleedingHealth.Value = Mathf.Max(0, _bleedingHealth.Value - value);
            }
            else if (mode == ChangeValueMode.Bleeding)
            {
                lastHP = _health.Value;
                _health.Value = Mathf.Max(0, _health.Value - value);
            }
        }

        /// <summary>
        /// 体力を変更させるための関数(最大体力の比率を用いて計算する用)
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">値変更のモードを指定</param>
        public void ChangeHealth(float value, ChangeValueMode mode)
        {
            if (mode == ChangeValueMode.Heal)
            {
                lastHP = _health.Value;
                _health.Value = Mathf.Min(100, _health.Value + (int)(_healthBase * value));
                _bleedingHealth.Value = Mathf.Min(100, _bleedingHealth.Value + (int)(_healthBase * value));

                //if (_networkObject.HasInputAuthority)
                SpellUnlockSystem.Instance.IncreaseHealedHealth(_health.Value - lastHP);
            }
            else if (mode == ChangeValueMode.Damage)
            {
                if (_isProtect)
                {
                    _isProtect = false;
                    Debug.Log("呪文によりダメージが無効化されました。");
                    return;
                }
                lastHP = _health.Value;
                _health.Value = Mathf.Max(0, _health.Value - (int)(_healthBase * value));
                _bleedingHealth.Value = Mathf.Max(0, _bleedingHealth.Value - (int)(_healthBase * value));
            }
            else if (mode == ChangeValueMode.Bleeding)
            {
                lastHP = _health.Value;
                _health.Value = Mathf.Max(0, _health.Value - (int)(_healthBase * value));
            }
        }


        /// <summary>
        /// 出血処理用体力を変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        public void ChangeBleedingHealth(int value)
        {
            _bleedingHealth.Value = Mathf.Max(0, _bleedingHealth.Value - value);
        }

        /// <summary>
        /// スタミナを変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">値変更のモードを指定</param>
        public void ChangeStamina(int value, ChangeValueMode mode)
        {
            if (mode == ChangeValueMode.Heal)
                _stamina.Value = Mathf.Min(100, _stamina.Value + value);
            else if (mode == ChangeValueMode.Damage)
                _stamina.Value = Mathf.Max(0, _stamina.Value - (int)(value * (_isBuffedAdrenaline ? 0.5f : 1)));
        }

        /// <summary>
        /// SAN値を変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">値変更のモードを指定</param>
        public void ChangeSanValue(int value, ChangeValueMode mode)
        {
            if (mode == ChangeValueMode.Heal)
            {
                lastSanValue = _san.Value;
                _san.Value = Mathf.Min(100, _san.Value + value);
            }

            else if (mode == ChangeValueMode.Damage)
            {
                lastSanValue = _san.Value;
                _san.Value = Mathf.Max(0, _san.Value - value / (_isHaveCharm ? 2 : 1));
            }
        }

        /// <summary>
        /// 移動速度を変更させる関数
        /// </summary>
        public void ChangeSpeed()
        {
            _speed.Value = (int)(_speedBase * (_isUseItem ? 0.5f : 1) * (_isUseMagic ? 0.5f : 1) * (_isUseEscapePoint ? 0.5f : 1));
        }

        /// <summary>
        /// アイテムを使っているのか管理するための関数
        /// </summary>
        /// <param name="value"></param>
        public void UseItem(bool value)
        {
            _isUseItem = value;
        }

        /// <summary>
        /// 呪文を唱えているか管理するための関数
        /// </summary>
        /// <param name="value"></param>
        public void UseMagic(bool value)
        {
            _isUseMagic = value;
        }

        /// <summary>
        /// お守りがアイテムスロットにあるか判定する関数
        /// </summary>
        /// <param name="value"></param>
        public void HaveCharm(bool value)
        {
            _isHaveCharm = value;
        }

        /// <summary>
        /// バリア呪文を使ったときに_isProtectをtrueにさせる関数
        /// </summary>
        /// <param name="value"></param>
        public void UseProtectMagic(bool value)
        { 
            _isProtect = value;
        }

        /// <summary>
        /// 呪文を唱えているか管理するための関数
        /// </summary>
        /// <param name="value"></param>
        public void UseEscapePoint(bool value,float time = 0)
        {
            if (value)
            {
                castEvent.OnNext(time);
            }
            _isUseEscapePoint = value;
        }

        /// <summary>
        /// 心拍数に応じて出血状態時の出血量を変化させる関数
        /// </summary>
        /// <param name="value"></param>
        public void PulsationBleeding(bool value)
        {
            _isPulsationBleeding = value;
        }

        /// <summary>
        /// _bleeding(出血状態)の値を変更するための関数
        /// </summary>
        /// <param name="value"></param>
        public void ChangeBleedingBool(bool value)
        {
            _bleeding.Value = value;
            if (value == false)
            {
                //if (_networkObject.HasInputAuthority)
                SpellUnlockSystem.Instance.IncreaseStopBleedingTimes();
            }
                
        }

        public void ChangePlayerActionState(PlayerActionState state)
        {
            _playerActionState.Value = state;
        }

        public void ReviveCharacter()
        {
            Debug.Log("ReviveCharacter起動");
            _survive.Value = true;
            ChangeHealth(50, ChangeValueMode.Heal);
        }

        /// <summary>
        /// 出血状態の処理を行う関数。
        /// </summary>
        /// <returns></returns>
        private IEnumerator Bleeding(int damage)
        {
            ChangeBleedingHealth(damage *(_isPulsationBleeding ? 2 : 1));
            for (int i = 0; i < damage; i++)
            {
                if (_bleeding.Value)
                {
                    ChangeHealth((_isPulsationBleeding ? 2 : 1), ChangeValueMode.Bleeding);
                    Debug.Log("出血ダメージが入りました。");

                    if (i == damage - 1)//最後の出血処理の後、すぐに出血Boolをfalseを変更させるため
                        break;
                }
                else
                    break;

                yield return new WaitForSeconds(1.0f);
            }
                ChangeBleedingBool(false);
                yield break;
        }

        /// <summary>
        /// 体力に関する処理を行う
        /// </summary>
        /// <param name="health">残り体力</param>
        private void CheckHealth(int health, int ID)
        {
            //Debug.Log("残り体力：" + health);


            if (health <= 0)
                _survive.Value = false;

            if(lastHP >= 0 && health < 0)
            {
                _survive.Value = true;
            }


        }

        /// <summary>
        /// スタミナに関する処理を行う
        /// </summary>
        /// <param name="stamina">残りスタミナ</param>
        private void CheckStamina(int stamina, int ID)
        {
            //スタミナ残量をゲーム内に表示.
            //Debug.Log("残りスタミナ：" + stamina);
        }

        /// <summary>
        /// san値に関する処理を行う
        /// </summary>
        /// <param name="sanValue">残りのSAN値</param>
        private void CheckSanValue(int sanValue, int ID)
        {
            //Debug.Log("残りsan値：" + sanValue);           

            if (sanValue <= 0)
                _survive.Value = false;
        }


        /// <summary>
        /// 生死状態の変更時に処理を行う
        /// </summary>
        /// <param name="isSurvive">生きているか否か</param>
        private void CheckSurvive(bool isSurvive)
        {
            Debug.Log("CheckSurvive起動");
            if (isSurvive)//生き返ったとき
            {
                //今後蘇生関連の仕様が上がったら処理を実行させる
                _anim.SetBool("Survive", true);               
                _playerItem.ChangeCanUseItem(true);
                if (!_playerMagic.GetUsedMagicBool())
                {
                    _playerMagic.ChangeCanUseMagicBool(true);
                }

                _deathEventCount = 0;
                _startReviveAnimation = true;
            }
            else //死んだとき
            {
                //死因に応じて死亡時アニメーションを変える
                if (_health.Value <= 0)//体力が0で死んだとき
                { 
                
                }
                else if(_san.Value <= 0)//SAN値が0で死んだとき
                {
                     
                }
                _anim.SetBool("Survive", false);
                _anim.SetBool("FinishRevive", false);
                _playerMagic.ChangeCanUseMagicBool(false);
                _playerItem.ChangeCanUseItem(false);
                _deathEventCount = 0;
            }
        }

        /// <summary>
        /// アニメーションイベント。死亡時に実行
        /// </summary>
        private void DeathAnimationBoolChange()
        {
            _deathEventCount += 1;
            _startDeathAnimation = true;

            if (_deathEventCount == 2)//2回目のイベント時のみ実行
            {
                _startDeathAnimation = !_startDeathAnimation;
                _playerItem.CheckHaveDoll();
            }

        }

        /// <summary>
        /// 蘇生アニメーションが終了したことを知らせる関数
        /// </summary>
        private void FinishReviveAnimation()
        {
            _anim.SetBool("FinishRevive", true);
            _startReviveAnimation = false;
        }

        public void StartBuff()
        {
            StartCoroutine(BuffAdrenaline());
        }
        
        /// <summary>
        /// アドレナリン上昇状態を変化させるための関数
        /// </summary>
        public IEnumerator BuffAdrenaline()
        {
            _isBuffedAdrenaline = true;
            yield return new WaitForSeconds(15.0f);
            _isBuffedAdrenaline = false;
            yield break;
        }


        /// <summary>
        /// 光の距離を変化させる関数
        /// </summary>
        /// <param name="extendLightRange"></param>光の距離の伸ばすか否か判定する変数
        public void ChangeLightRange(bool extendLightRange)
        {
            _lightrange.Value = (extendLightRange ? 80 : 20);
        }

    }
}

