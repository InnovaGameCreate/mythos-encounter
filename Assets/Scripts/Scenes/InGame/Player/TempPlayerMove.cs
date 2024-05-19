�ｿusing Scenes.Ingame.Player;
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
        Vector3 _moveVelocity;

        [Header("繧ｫ繝｡繝ｩ髢｢菫")]
        [SerializeField] private CameraMove _myCameraMove;
        [SerializeField] private GameObject _camera;
        [SerializeField] private bool isCurcleSetting;
        private Vector3 _nowCameraAngle;

        [SerializeField] private float moveSpeed;
        [Tooltip("繧ｹ繧ｿ繝溘リ縺ｮ蝗槫ｾｩ驥(per 1sec)")][SerializeField] private int _recoverStamina = 20;
        [Tooltip("繧ｹ繧ｿ繝溘リ縺ｮ蝗槫ｾｩ驥充逍ｲ蜉ｴ譎�(per 1sec)")][SerializeField] private int _recoverStaminaOnlyTired = 60;
        [Tooltip("繧ｹ繧ｿ繝溘リ縺ｮ豸郁ｲｻ驥(per 1sec)")][SerializeField] private int _expandStamina = 20;

        private bool _isTiredPenalty = false;
        private bool _isCanMove = true;
        private bool _isCannotMoveByParalyze = false;
        private PlayerActionState _lastPlayerAction = PlayerActionState.Idle;

        //主に外部スクリプトで扱うフィールド
        private bool _isParalyzed = false;//身体の麻痺.BodyParalyze.Csで使用
        private bool _isPulsation = false;//心拍数増加.IncreasePulsation.Csで使用

        void Start()
        {
            if (isCurcleSetting)
                CursorSetting();

            _nowCameraAngle = _camera.transform.localEulerAngles;

            //繧ｭ繝ｼ繝舌う繝ｳ繝峨�險ｭ螳
            KeyCode dash = KeyCode.LeftShift;
            KeyCode sneak = KeyCode.LeftControl;

            #region Subscribes
            //繝励Ξ繧､繝､繝ｼ縺ｮ蝓ｺ遉朱溷ｺｦ縺悟､画峩縺輔ｌ縺溘ｉ
            _myPlayerStatus.OnPlayerSpeedChange
                .Where(x => x >= 0)
                .Subscribe(x => moveSpeed = x).AddTo(this);

            //繝励Ξ繧､繝､繝ｼ縺ｮ陦悟虚迥ｶ諷九′螟牙喧縺励◆繧
            _myPlayerStatus.OnPlayerActionStateChange
                .Skip(1)//蛻晏屓�医せ繝昴�繝ｳ逶ｴ蠕鯉ｼ峨�陦後ｏ縺ｪ縺
                .Where(state => state == PlayerActionState.Idle || state == PlayerActionState.Walk || state == PlayerActionState.Dash || state == PlayerActionState.Sneak)
                .Subscribe(state =>
                {
                    //繧ｹ繧ｿ繝溘リ縺ｮ蠅玲ｸ帙ｒ豎ｺ螳
                    if (state == PlayerActionState.Dash)
                        StartCoroutine(DecreaseStamina());
                    else if(_lastPlayerAction == PlayerActionState.Dash && state != PlayerActionState.Dash)//螟牙喧蜑阪�迥ｶ諷九′繝繝�す繝･縺ｧ縺九▽縲∝､牙喧蠕後′繧ｹ繧ｿ繝溘リ繧貞屓蠕ｩ縺ｧ縺阪ｋ迥ｶ諷九�譎
                        StartCoroutine(IncreaseStamina());                                                 //繧ｹ繧ｿ繝溘リ蝗槫ｾｩ繧ｳ繝ｫ繝ｼ繝√Φ縺ｮ驥崎､�ｒ驕ｿ縺代ｋ縺溘ａ縺ｮ蜃ｦ鄂ｮ

                    //雜ｳ髻ｳ縺ｮ遞ｮ鬘槭ｒ豎ｺ螳壹�魑ｴ繧峨☆
                    _myPlayerSoundManager.FootSound(state);
                    //遘ｻ蜍輔↓繧医ｋ隕也せ縺ｮ螟牙喧縺ｮ莉墓婿繧定ｨｭ螳
                    _myCameraMove.ChangeViewPoint(_myPlayerSoundManager.GetClipLength());
                }).AddTo(this);

            //蠕�ｩ溽憾諷九↓蛻�ｊ譖ｿ縺
            //菴輔ｂ蜈･蜉帙＠縺ｦ縺�↑縺 or WS繧ｭ繝ｼ縺ｮ蜷梧凾謚ｼ縺励�繧医≧縺ｫ莠偵＞縺ｫ謇薙■豸医＠縺ｦ蜍輔°縺ｪ縺�→縺阪↓蛻�ｊ譖ｿ縺医ｋ
            this.UpdateAsObservable()
                .Where(_ =>!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) ||
                           _lastPlayerAction != PlayerActionState.Idle && _moveVelocity == Vector3.zero)
                .Subscribe(_ => 
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//螟牙喧蜑阪�迥ｶ諷九ｒ險倬鹸縺吶ｋ縲
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Idle);
                });

            //繧ｭ繝ｼ蜈･蜉帙�迥ｶ豕√↓繧医ｋ豁ｩ陦檎憾諷九∈縺ｮ蛻�ｊ譖ｿ縺
            //竭繝繝�す繝･繧ｭ繝ｼ繧呈款縺励※縺�↑縺,繧ｹ繝九�繧ｯ繧ｭ繝ｼ繧呈款縺励※縺�↑縺,遘ｻ蜍墓婿蜷代�繧ｯ繝医Ν縺0縺ｧ縺ｪ縺,WASD縺ｩ繧後°縺ｯ謚ｼ縺励※縺�ｋ縲ゅ％繧後ｉ繧呈ｺ縺溘＠縺溘→縺
            //竭｡襍ｰ縺｣縺ｦ縺�ｋ迥ｶ諷九〒W繧ｭ繝ｼ繧帝屬縺励◆縺ｨ縺
            this.UpdateAsObservable()
                .Where(_ => (!Input.GetKey(dash) && !Input.GetKey(sneak) && _moveVelocity != Vector3.zero &&
                            (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) ) ||
                             (_myPlayerStatus.nowPlayerActionState == PlayerActionState.Dash && !Input.GetKey(KeyCode.W)) )
                .Where(_ => _isCanMove && !_isCannotMoveByParalyze)
                .Subscribe(_ => 
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//螟牙喧蜑阪�迥ｶ諷九ｒ險倬鹸縺吶ｋ縲
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);
                });

            //繧ｹ繧ｿ繝溘リ縺悟�繧後◆髫帙�豁ｩ陦檎憾諷九∈縺ｮ蛻�ｊ譖ｿ縺茨ｼ医�繝翫Ν繝�ぅ縺後▽縺擾ｼ
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(dash) && Input.GetKey(KeyCode.W) && _myPlayerStatus.nowStaminaValue == 0)
                .Subscribe(_ =>
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//螟牙喧蜑阪�迥ｶ諷九ｒ險倬鹸縺吶ｋ縲
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);
                    StartCoroutine(CountTiredPenalty());
                });

            //Shift+遘ｻ蜍輔く繝ｼ繧呈款縺励◆縺ｨ縺阪ム繝�す繝･迥ｶ諷九↓蛻�ｊ譖ｿ縺
            this.UpdateAsObservable()
                .Where(_ => ((Input.GetKeyDown(dash) && Input.GetKey(KeyCode.W)) || (Input.GetKey(dash) && Input.GetKeyDown(KeyCode.W))) && !_isTiredPenalty && _moveVelocity != Vector3.zero)
                .Where(_ => _isCanMove && !_isCannotMoveByParalyze)
                .Subscribe(_ => 
                {
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Dash);
                });

            //Ctrl+遘ｻ蜍輔く繝ｼ繧呈款縺励◆縺ｨ縺榊ｿ阪�豁ｩ縺咲憾諷九↓蛻�ｊ譖ｿ縺
            this.UpdateAsObservable()
                .Where(_ => (Input.GetKeyDown(sneak) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))) ||
                            (Input.GetKey(sneak) && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
                            && _moveVelocity != Vector3.zero)
                .Where(_ => _isCanMove && !_isCannotMoveByParalyze)
                .Subscribe(_ =>
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//螟牙喧蜑阪�迥ｶ諷九ｒ險倬鹸縺吶ｋ縲
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Sneak);
                });
            #endregion

            StartCoroutine(CheckParalyze());
        }

        /// <summary>
        /// 繧ｫ繝ｼ繧ｽ繝ｫ縺ｮ險ｭ螳壹ｒ縺励※縺上ｌ繧
        /// </summary>
        private void CursorSetting()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            float moveMouseX = Input.GetAxis("Mouse X");
            if (Mathf.Abs(moveMouseX) > 0.001f)
            {
                // 蝗櫁ｻ｢霆ｸ縺ｯ繝ｯ繝ｼ繝ｫ繝牙ｺｧ讓吶�Y霆ｸ
                transform.RotateAround(transform.position, Vector3.up, moveMouseX);
            }

            //繧ｫ繝｡繝ｩ繧湛霆ｸ譁ｹ蜷代↓蝗櫁ｻ｢縺輔○繧九りｦ也せ縺御ｸ贋ｸ九↓蜍輔°縺帙ｋ繧医≧縺ｫ�育ｯ�峇縺ｫ蛻ｶ髯舌≠繧奇ｼ
            float moveMouseY = Input.GetAxis("Mouse Y");
            if (Mathf.Abs(moveMouseY) > 0.001f)
            {
                _nowCameraAngle.x -= moveMouseY;
                _nowCameraAngle.x = Mathf.Clamp(_nowCameraAngle.x, -40, 60);
                _camera.gameObject.transform.localEulerAngles = _nowCameraAngle;
            }

            //動ける状態であれば動く
            if (_isCanMove && !_isCannotMoveByParalyze)
                Move();
            else if(!_isCanMove || _isCannotMoveByParalyze)
            {
                _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//変化前の状態を記録する。
                _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Idle);//待機状態へ移行
            }

            //閾ｪ逕ｱ關ｽ荳
            if (this.gameObject.transform.position.y > 0)
                this.gameObject.transform.position -= new Vector3(0, 9.8f *Time.deltaTime, 0);
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

            //迥ｶ諷九↓蠢懊§縺ｦ遘ｻ蜍暮溷ｺｦ縺悟､牙喧
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
                _myPlayerStatus.ChangeStamina(_expandStamina / 10 * (_isPulsation ? 2 : 1), "Damage");
            }           
        }

        private IEnumerator IncreaseStamina()
        {
            yield return null;
            
            //繝�ヵ繧ｩ繝ｫ繝育憾諷九〒縺ｮ蝗槫ｾｩ
            if(!_isTiredPenalty)
            {
                while (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Dash)
                {
                    yield return new WaitForSeconds(0.1f);
                    _myPlayerStatus.ChangeStamina(_recoverStamina / 10, "Heal");
                }

            }
            //繧ｹ繧ｿ繝溘リ蛻�ｌ迥ｶ諷九〒縺ｮ蝗槫ｾｩ
            else縲if(_isTiredPenalty)
            {
                yield return new WaitForSeconds(0.5f);//蝗槫ｾｩ髢句ｧ九∪縺ｧ縺ｮ繝ｩ繧ｰ

                while (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Dash)
                {
                    yield return new WaitForSeconds(0.1f);
                    _myPlayerStatus.ChangeStamina(_recoverStaminaOnlyTired / 10, "Heal");
                }
            }

        }

        private IEnumerator CountTiredPenalty()
        { 
            _isTiredPenalty = true;
            yield return new WaitUntil(() => _myPlayerStatus.nowStaminaValue縲> 10);//繧ｹ繧ｿ繝溘リ縺10縺ｫ縺ｪ繧九∪縺ｧ蝗槫ｾｩ縺吶ｋ縺ｮ繧貞ｾ�▽
            _isTiredPenalty = false;
        }

        private IEnumerator CheckParalyze()
        { 
            while (true) 
            {
                yield return new WaitForSeconds(5.0f);
                if (_isParalyzed)
                {
                    //25%の確率で1秒間動けない
                    int random = Random.Range(0, 4);
                    if (random == 0)
                    {
                        _isCannotMoveByParalyze = true;
                        Debug.Log("体が思うように動かない...!!");
                    }
                    else
                    {
                        _isCannotMoveByParalyze = false;
                        Debug.Log("動ける!!");
                    }                       
                }
            }
        }

        /// <summary>
        /// 体が麻痺しているか否かを決定する関数
        /// </summary>
        /// <param name="value"></param>
        public void Paralyze(bool value)
        {
            _isParalyzed = value;

            //麻痺状態が治ってたら、動けるようにもする
            if (value == false)
                _isCannotMoveByParalyze = false;
        }

        /// <summary>
        /// 心拍数が増えているか否かを決定する関数
        /// </summary>
        /// <param name="value"></param>
        public void Pulsation(bool value)
        {
            _isPulsation = value;
        }

        /// <summary>
        /// 移動できるか否かを決定する関数
        /// </summary>
        /// <param name="value"></param>
        public void MoveControl(bool value)
        { 
            _isCanMove = value;
        }


    }
}


