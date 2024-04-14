using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Scenes.Ingame.Player
{
    public class TempPlayerMove : MonoBehaviour
    {
        [SerializeField] CharacterController _characterController;
        [SerializeField] private PlayerStatus _myPlayerStatus;
        [SerializeField] private PlayerSoundManager _myPlayerSoundManager;
        [SerializeField] private CameraMove _myCameraMove;
        Vector3 _moveVelocity;

        [SerializeField] private float moveSpeed;
        [Tooltip("スタミナの回復量(per 1sec)")][SerializeField] private int _recoverStamina;
        [Tooltip("スタミナの消費量(per 1sec)")][SerializeField] private int _expandStamina;

        private bool _isTiredPenalty = false;
        private PlayerActionState _lastPlayerAction = PlayerActionState.Idle;

        void Start()
        {
            //Cursor.visible = false;

            //キーバインドの設定
            KeyCode dash = KeyCode.LeftShift;
            KeyCode sneak = KeyCode.LeftControl;

            #region Subscribes
            //プレイヤーの基礎速度が変更されたら
            _myPlayerStatus.OnPlayerSpeedChange
                .Where(x => x >= 0)
                .Subscribe(x => moveSpeed = x).AddTo(this);

            //プレイヤーの行動状態が変化したら
            _myPlayerStatus.OnPlayerActionStateChange
                .Where(state => state == PlayerActionState.Idle || state == PlayerActionState.Walk || state == PlayerActionState.Dash || state == PlayerActionState.Sneak)
                .Subscribe(state =>
                {
                    //スタミナの増減を決定
                    if (state == PlayerActionState.Dash)
                        StartCoroutine(DecreaseStamina());
                    else if(_lastPlayerAction == PlayerActionState.Dash && state != PlayerActionState.Dash)//変化前の状態がダッシュでかつ、変化後がスタミナを回復できる状態の時
                        StartCoroutine(IncreaseStamina());                                                 //スタミナ回復コルーチンの重複を避けるための処置

                    //足音の種類を決定・鳴らす
                    _myPlayerSoundManager.FootSound(state);
                    //移動による視点の変化の仕方を設定
                    _myCameraMove.ChangeViewPoint(_myPlayerSoundManager.GetClipLength());
                }).AddTo(this);

            //待機状態に切り替え
            //何も入力していない or WSキーの同時押しのように互いに打ち消して動かないときに切り替える
            this.UpdateAsObservable()
                .Where(_ =>!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) ||
                           _lastPlayerAction != PlayerActionState.Idle && _moveVelocity == Vector3.zero)
                .Subscribe(_ => 
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//変化前の状態を記録する。
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Idle);
                });

            //キー入力の状況による歩行状態への切り替え
            //�@ダッシュキーを押していない,スニークキーを押していない,移動方向ベクトルが0でない,WASDどれかは押している。これらを満たしたとき
            //�A走っている状態でWキーを離したとき
            this.UpdateAsObservable()
                .Where(_ => (!Input.GetKey(dash) && !Input.GetKey(sneak) && _moveVelocity != Vector3.zero &&
                            (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) ) ||
                             (_myPlayerStatus.nowPlayerActionState == PlayerActionState.Dash && !Input.GetKey(KeyCode.W)) )  
                .Subscribe(_ => 
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//変化前の状態を記録する。
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);
                });

            //スタミナが切れた際の歩行状態への切り替え（ペナルティがつく）
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(dash) && Input.GetKey(KeyCode.W) && _myPlayerStatus.nowStaminaValue == 0)
                .Subscribe(_ =>
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//変化前の状態を記録する。
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);
                    StartCoroutine(CountTiredPenalty());
                });

            //Shift+移動キーを押したときダッシュ状態に切り替え
            this.UpdateAsObservable()
                .Where(_ => ((Input.GetKeyDown(dash) && Input.GetKey(KeyCode.W)) || (Input.GetKey(dash) && Input.GetKeyDown(KeyCode.W))) && !_isTiredPenalty && _moveVelocity != Vector3.zero)
                .Subscribe(_ => 
                {
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Dash);
                });

            //Ctrl+移動キーを押したとき忍び歩き状態に切り替え
            this.UpdateAsObservable()
                .Where(_ => (Input.GetKeyDown(sneak) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))) ||
                            (Input.GetKey(sneak) && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
                            && _moveVelocity != Vector3.zero)
                .Subscribe(_ =>
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//変化前の状態を記録する。
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Sneak);
                });
            #endregion

        }

        void Update()
        {
            float moveMouseX = Input.GetAxis("Mouse X");
            if (Mathf.Abs(moveMouseX) > 0.001f)
            {
                // 回転軸はワールド座標のY軸
                transform.RotateAround(transform.position, Vector3.up, moveMouseX);
            }

            Move();
        }

        private void Move()
        {
            _moveVelocity = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                _moveVelocity += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _moveVelocity -= transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                _moveVelocity -= transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _moveVelocity += transform.right;
            }
            _moveVelocity = _moveVelocity.normalized;

            //状態に応じて移動速度や足音が変化
            switch (_myPlayerStatus.nowPlayerActionState)
            {
                case PlayerActionState.Walk:
                    _characterController.Move(_moveVelocity * Time.deltaTime * moveSpeed);
                    break;
                case PlayerActionState.Dash:
                    _characterController.Move(_moveVelocity * Time.deltaTime * moveSpeed * 2);
                    break;
                case PlayerActionState.Sneak:
                    _characterController.Move(_moveVelocity * Time.deltaTime * moveSpeed / 2);
                    break;
                default:
                    break;
            }            
        }

        private IEnumerator DecreaseStamina()
        {
            while (_myPlayerStatus.nowPlayerActionState == PlayerActionState.Dash)
            { 
                yield return new WaitForSeconds(0.1f);
                _myPlayerStatus.ChangeStamina(_recoverStamina / 10, "Damage");
            }           
        }

        private IEnumerator IncreaseStamina()
        {
            while (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Dash)
            {
                yield return new WaitForSeconds(0.1f);
                _myPlayerStatus.ChangeStamina(_expandStamina / 10, "Heal");
            }
        }

        private IEnumerator CountTiredPenalty()
        { 
            _isTiredPenalty = true;
            yield return new WaitUntil(() => _myPlayerStatus.nowStaminaValue > 10);//スタミナが10まで回復するのを待つ
            _isTiredPenalty = false;
        }
    }
}


