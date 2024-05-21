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
        Vector3 _moveVelocity;

        [Header("�J�����֌W")]
        [SerializeField] private CameraMove _myCameraMove;
        [SerializeField] private GameObject _camera;
        [SerializeField] private bool isCurcleSetting;
        private Vector3 _nowCameraAngle;

        [SerializeField] private float moveSpeed;
        [Tooltip("�X�^�~�i�̉񕜗�(per 1sec)")][SerializeField] private int _recoverStamina;
        [Tooltip("�X�^�~�i�̉񕜗�[�X�^�~�i�؂ꎞ](per 1sec)")][SerializeField] private int _recoverStaminaOnlyTired;
        [Tooltip("�X�^�~�i�̏����(per 1sec)")][SerializeField] private int _expandStamina;

        private bool _isTiredPenalty = false;
        private bool _isCanMove = true;
        private bool _isCannotMoveByParalyze = false;
        private PlayerActionState _lastPlayerAction = PlayerActionState.Idle;

        //��ɊO���X�N���v�g�ň����t�B�[���h
        private bool _isParalyzed = false;//�g�̖̂��.BodyParalyze.Cs�Ŏg�p
        private bool _isPulsation = false;//�S��������.IncreasePulsation.Cs�Ŏg�p

        void Start()
        {
            if (isCurcleSetting)
                CursorSetting();

            _nowCameraAngle = _camera.transform.localEulerAngles;

            //�L�[�o�C���h�̐ݒ�
            KeyCode dash = KeyCode.LeftShift;
            KeyCode sneak = KeyCode.LeftControl;

            #region Subscribes
            //�v���C���[�̊�b���x���ύX���ꂽ��
            _myPlayerStatus.OnPlayerSpeedChange
                .Where(x => x >= 0)
                .Subscribe(x => moveSpeed = x).AddTo(this);

            //�v���C���[�̍s����Ԃ��ω�������
            _myPlayerStatus.OnPlayerActionStateChange
                .Skip(1)//����i�X�|�[������j�͍s��Ȃ�
                .Where(state => state == PlayerActionState.Idle || state == PlayerActionState.Walk || state == PlayerActionState.Dash || state == PlayerActionState.Sneak)
                .Subscribe(state =>
                {
                    //�X�^�~�i�̑���������
                    if (state == PlayerActionState.Dash)
                        StartCoroutine(DecreaseStamina());
                    else if(_lastPlayerAction == PlayerActionState.Dash && state != PlayerActionState.Dash)//�ω��O�̏�Ԃ��_�b�V���ł��A�ω��オ�X�^�~�i���񕜂ł����Ԃ̎�
                        StartCoroutine(IncreaseStamina());                                                 //�X�^�~�i�񕜃R���[�`���̏d��������邽�߂̏��u

                    //�����̎�ނ�����E�炷
                    _myPlayerSoundManager.FootSound(state);
                    //�ړ��ɂ�鎋�_�̕ω��̎d����ݒ�
                    _myCameraMove.ChangeViewPoint(_myPlayerSoundManager.GetClipLength());
                }).AddTo(this);

            //�ҋ@��Ԃɐ؂�ւ�
            //�������͂��Ă��Ȃ� or WS�L�[�̓��������̂悤�Ɍ݂��ɑł������ē����Ȃ��Ƃ��ɐ؂�ւ���
            this.UpdateAsObservable()
                .Where(_ =>!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) ||
                           _lastPlayerAction != PlayerActionState.Idle && _moveVelocity == Vector3.zero)
                .Subscribe(_ => 
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//�ω��O�̏�Ԃ��L�^����B
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Idle);
                });

            //�L�[���͂̏󋵂ɂ����s��Ԃւ̐؂�ւ�
            //�@�_�b�V���L�[�������Ă��Ȃ�,�X�j�[�N�L�[�������Ă��Ȃ�,�ړ������x�N�g����0�łȂ�,WASD�ǂꂩ�͉����Ă���B�����𖞂������Ƃ�
            //�A�����Ă����Ԃ�W�L�[�𗣂����Ƃ�
            this.UpdateAsObservable()
                .Where(_ => (!Input.GetKey(dash) && !Input.GetKey(sneak) && _moveVelocity != Vector3.zero &&
                            (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) ) ||
                             (_myPlayerStatus.nowPlayerActionState == PlayerActionState.Dash && !Input.GetKey(KeyCode.W)) )
                .Where(_ => _isCanMove && !_isCannotMoveByParalyze)
                .Subscribe(_ => 
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//�ω��O�̏�Ԃ��L�^����B
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);
                });

            //�X�^�~�i���؂ꂽ�ۂ̕��s��Ԃւ̐؂�ւ��i�y�i���e�B�����j
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(dash) && Input.GetKey(KeyCode.W) && _myPlayerStatus.nowStaminaValue == 0)
                .Subscribe(_ =>
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//�ω��O�̏�Ԃ��L�^����B
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);
                    StartCoroutine(CountTiredPenalty());
                });

            //Shift+�ړ��L�[���������Ƃ��_�b�V����Ԃɐ؂�ւ�
            this.UpdateAsObservable()
                .Where(_ => ((Input.GetKeyDown(dash) && Input.GetKey(KeyCode.W)) || (Input.GetKey(dash) && Input.GetKeyDown(KeyCode.W))) && !_isTiredPenalty && _moveVelocity != Vector3.zero)
                .Where(_ => _isCanMove && !_isCannotMoveByParalyze)
                .Subscribe(_ => 
                {
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Dash);
                });

            //Ctrl+�ړ��L�[���������Ƃ��E�ѕ�����Ԃɐ؂�ւ�
            this.UpdateAsObservable()
                .Where(_ => (Input.GetKeyDown(sneak) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))) ||
                            (Input.GetKey(sneak) && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
                            && _moveVelocity != Vector3.zero)
                .Where(_ => _isCanMove && !_isCannotMoveByParalyze)
                .Subscribe(_ =>
                {
                    _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//�ω��O�̏�Ԃ��L�^����B
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Sneak);
                });
            #endregion

            StartCoroutine(CheckParalyze());
        }

        /// <summary>
        /// �J�[�\���̐ݒ�����Ă����
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
                // ��]���̓��[���h���W��Y��
                transform.RotateAround(transform.position, Vector3.up, moveMouseX);
            }

            //�J������X�������ɉ�]������B���_���㉺�ɓ�������悤�Ɂi�͈͂ɐ�������j
            float moveMouseY = Input.GetAxis("Mouse Y");
            if (Mathf.Abs(moveMouseY) > 0.001f)
            {
                _nowCameraAngle.x -= moveMouseY;
                _nowCameraAngle.x = Mathf.Clamp(_nowCameraAngle.x, -40, 60);
                _camera.gameObject.transform.localEulerAngles = _nowCameraAngle;
            }

            //�������Ԃł���Γ���
            if (_isCanMove && !_isCannotMoveByParalyze)
                Move();
            else if(!_isCanMove || _isCannotMoveByParalyze)
            {
                _lastPlayerAction = _myPlayerStatus.nowPlayerActionState;//�ω��O�̏�Ԃ��L�^����B
                _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Idle);//�ҋ@��Ԃֈڍs
            }

            //���R����
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

            //��Ԃɉ����Ĉړ����x���ω�
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

            if(!_isTiredPenalty)
            {
                while (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Dash)
                {
                    yield return new WaitForSeconds(0.1f);
                    _myPlayerStatus.ChangeStamina(_recoverStamina / 10, "Heal");
                }
            }
            else if(_isTiredPenalty)
            {
                yield return new WaitForSeconds(0.5f);
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
            yield return new WaitUntil(() => _myPlayerStatus.nowStaminaValue == 100);//�X�^�~�i��100�܂ŉ񕜂���̂�҂�
            _isTiredPenalty = false;
        }

        private IEnumerator CheckParalyze()
        { 
            while (true) 
            {
                yield return new WaitForSeconds(5.0f);
                if (_isParalyzed)
                {
                    //25%�̊m����1�b�ԓ����Ȃ�
                    int random = Random.Range(0, 4);
                    if (random == 0)
                    {
                        _isCannotMoveByParalyze = true;
                        Debug.Log("�̂��v���悤�ɓ����Ȃ�...!!");
                    }
                    else
                    {
                        _isCannotMoveByParalyze = false;
                        Debug.Log("������!!");
                    }                       
                }
            }
        }

        /// <summary>
        /// �̂���Ⴢ��Ă��邩�ۂ������肷��֐�
        /// </summary>
        /// <param name="value"></param>
        public void Paralyze(bool value)
        {
            _isParalyzed = value;

            //��჏�Ԃ������Ă���A������悤�ɂ�����
            if (value == false)
                _isCannotMoveByParalyze = false;
        }

        /// <summary>
        /// �S�����������Ă��邩�ۂ������肷��֐�
        /// </summary>
        /// <param name="value"></param>
        public void Pulsation(bool value)
        {
            _isPulsation = value;
        }

        /// <summary>
        /// �ړ��ł��邩�ۂ������肷��֐�
        /// </summary>
        /// <param name="value"></param>
        public void MoveControl(bool value)
        { 
            _isCanMove = value;
        }


    }
}


