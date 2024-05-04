using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// プレイヤーのステータスを管理するクラス
/// MV(R)PにおけるModelの役割を想定
/// </summary>
namespace Scenes.Ingame.Player
{

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

        //その他のSubject
        private Subject<Unit> _enemyAttackedMe = new Subject<Unit>();//敵から攻撃を食らったときのイベント

        //それぞれの購読側を公開する。他のClassでSubscribeできる。
        public IObservable<int> OnPlayerHealthChange { get { return _health; } }//_health(体力)が変化した際にイベントが発行
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

        public IObservable<Unit> OnEnemyAttackedMe { get { return _enemyAttackedMe; } }//敵から攻撃を受けた際にイベントが発行

        //一部情報の開示
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

        public int lastHP;//HPの変動前の数値を記録。比較に用いる
        public int bleedingDamage = 1;//出血時に受けるダメージ
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
            //初期化
            Init();
            _health.Subscribe(x => CheckHealth(x,_playerID));//体力が変化したときにゲーム内で変更を加える
            _stamina.Subscribe(x => CheckStamina(x, _playerID));//スタミナが変化したときにゲーム内で変更を加える
            _san.Subscribe(x => CheckSanValue(x, _playerID));//SAN値が変化したときにゲーム内で変更を加える
            _bleeding.
                Where(x => x == true).
                Subscribe(_ => StartCoroutine(Bleeding(bleedingDamage)));//出血状態になったときに出血処理を開始
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //デバッグ用.(必要無くなれば消す)
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
        /// 体力を変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">Heal(回復), Damage(減少)の二つのみ</param>
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
        /// スタミナを変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">Heal(回復), Damage(減少)の二つのみ</param>
        public void ChangeStamina(int value, string mode)
        {
            if (mode == "Heal")
                _stamina.Value = Mathf.Min(100, _stamina.Value + value);
            else if (mode == "Damage")
                _stamina.Value = Mathf.Max(0, _stamina.Value - value);
        }

        /// <summary>
        /// SAN値を変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">Heal(回復), Damage(減少)の二つのみ</param>
        public void ChangeSanValue(int value, string mode)
        {
            if (mode == "Heal")
                _san.Value = Mathf.Min(100, _san.Value + value);
            else if (mode == "Damage")
                _san.Value = Mathf.Max(0, _san.Value - value);
        }

        /// <summary>
        /// 移動速度を変更させる関数
        /// </summary>
        public void ChangeSpeed()
        {
            _speed.Value = (int)(_speedBase * (_isUseItem ? 0.5f : 1));
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
        /// _bleeding(出血状態)の値を変更するための関数
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
        /// 出血状態の処理を行う関数。
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
                    Debug.Log("出血ダメージが入りました。");
                }                   
                else 
                    yield break;
            }
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
        /// <param name="san">残りのSAN値</param>
        private void CheckSanValue(int sanValue, int ID)
        {
            //Debug.Log("残りsan値：" + sanValue);

            if (sanValue <= 0)
                _survive.Value = false;
        }
    }
}

