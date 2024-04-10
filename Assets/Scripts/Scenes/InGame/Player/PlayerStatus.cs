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

        public int playerID { get { return _playerID; } }

        //現在のステータスの変数（今後ネットワーク化予定）
        //初期化についても今後はデータベースを参照して行うようにする。
        [SerializeField] private IntReactiveProperty _health = new IntReactiveProperty();//HP.ゼロになると死亡
        [SerializeField] private IntReactiveProperty _stamina = new IntReactiveProperty();//スタミナ
        [SerializeField] private IntReactiveProperty _san = new IntReactiveProperty();//SAN値
        [SerializeField] private BoolReactiveProperty _survive = new BoolReactiveProperty(true);//生死.trueのときは生きている
        [SerializeField] private BoolReactiveProperty _bleeding = new BoolReactiveProperty(false);//出血状態.trueのときに時間経過で体力が減少


        //それぞれの購読側を公開する。他のClassでSubscribeできる。
        public IObservable<int> OnPlayerHealthChange { get { return _health; } }//_health(体力)が変化した際にイベントが発行
        public IObservable<int> OnPlayerStaminaChange { get { return _stamina; } }//_stamina(スタミナ)が変化した際にイベントが発行
        public IObservable<int> OnPlayerSanValueChange { get { return _san; } }//_san(SAN値)が変化した際にイベントが発行
        public IObservable<bool> OnPlayerSurviveChange { get { return _survive; } }//_survive(生死)が変化した際にイベントが発行
        public IObservable<bool> OnPlayerbleedingChange { get { return _bleeding; } }//_bleeding(出血状態)が変化した際にイベントが発行


        //[SerializeField] private DisplayPlayerStatusManager _displayPlayerStatusManager;

        private void Init()
        {
            _health.Value = _healthBase;
            _stamina.Value = _staminaBase;
            _san.Value = _sanBase;
        }
        // Start is called before the first frame update
        void Start()
        {
            //初期化
            Init();
            _health.Subscribe(x => CheckHealth(x,_playerID));//体力が変化したときにゲーム内で変更を加える
            _stamina.Subscribe(x => CheckStamina(x, _playerID));//スタミナが変化したときにゲーム内で変更を加える
            _san.Subscribe(x => CheckSanValue(x, _playerID));//SAN値が変化したときにゲーム内で変更を加える
            _bleeding.
                Where(x => x == true).
                Subscribe(_ => StartCoroutine(Bleeding()));//出血状態になったときに出血処理を開始
        }

        // Update is called once per frame
        void Update()
        {
            //デバッグ用.
            if (Input.GetKeyDown(KeyCode.L))
            {
                ChangeHealth(20, "damage");
                ChangeStamina(20, "damage");
                ChangeSanValue(20, "damage");
                ChangeBleedingBool(true);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                ChangeBleedingBool(false);
            }
        }

        /// <summary>
        /// 体力を変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">Heal(回復), damage(減少)の二つのみ</param>
        public void ChangeHealth(int value, string mode)
        {
            if (mode == "Heal")
                _health.Value = Mathf.Min(100, _health.Value + value); 

            else if(mode == "damage")
                _health.Value = Mathf.Max(0, _health.Value - value);

        }

        /// <summary>
        /// スタミナを変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">Heal(回復), damage(減少)の二つのみ</param>
        public void ChangeStamina(int value, string mode)
        {
            if (mode == "Heal")
                _stamina.Value = Mathf.Min(100, _stamina.Value + value);
            else if (mode == "damage")
                _stamina.Value = Mathf.Max(0, _stamina.Value - value);
        }

        /// <summary>
        /// SAN値を変更させるための関数
        /// </summary>
        /// <param name="value">変更量</param>
        /// <param name="mode">Heal(回復), damage(減少)の二つのみ</param>
        public void ChangeSanValue(int value, string mode)
        {
            if (mode == "Heal")
                _san.Value = Mathf.Min(100, _san.Value + value);
            else if (mode == "damage")
                _san.Value = Mathf.Max(0, _san.Value - value);
        }

        /// <summary>
        /// _bleeding(出血状態)の値を変更するための関数
        /// </summary>
        /// <param name="value"></param>
        public void ChangeBleedingBool(bool value)
        {
            _bleeding.Value = value;
        }

        /// <summary>
        /// 出血状態の処理を行う関数。
        /// </summary>
        /// <returns></returns>
        private IEnumerator Bleeding()
        {
            while (_bleeding.Value)
            {
                yield return new WaitForSeconds(1.0f);
                if (_bleeding.Value) 
                    ChangeHealth(1, "damage");
                else 
                    break;
            }
        }

        /// <summary>
        /// 体力に関する処理を行う
        /// </summary>
        /// <param name="health">残り体力</param>
        private void CheckHealth(int health, int ID)
        {
            Debug.Log("残り体力：" + health);

            
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
            Debug.Log("残りスタミナ：" + stamina);
        }

        /// <summary>
        /// san値に関する処理を行う
        /// </summary>
        /// <param name="san">残りのSAN値</param>
        private void CheckSanValue(int sanValue, int ID)
        {
            Debug.Log("残りsan値：" + sanValue);

            if (sanValue <= 0)
                _survive.Value = false;
        }


    }
}

