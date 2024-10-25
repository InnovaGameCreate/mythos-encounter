using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UniRx;

using Fusion;
using Fusion.Addons.SimpleKCC;

using Cinemachine;



namespace Scenes.Ingame.Player
{
    public class MultiPlayerMove : NetworkBehaviour
    {
        Dictionary<PlayerActionState, float> _ajustValueOfState = new()
        {
            [PlayerActionState.Idle] = 0,
            [PlayerActionState.Walk] = 1,
            [PlayerActionState.Dash] = 1.5f,
            [PlayerActionState.Sneak] = 0.5f
        };


        Vector3 _moveVelocity;
        float _moveAdjustValue;


        [Header("�Q��")]
        [SerializeField] Transform CameraPivot;
        [SerializeField] NetworkCharacterController _multiCharacterController;
        [SerializeField] SimpleKCC _simpleKCC;
        [SerializeField] PlayerInput _input;
        [SerializeField] PlayerStatus _myPlayerStatus;

        [Header("�ړ�")]
        [SerializeField] float moveSpeed;
        [SerializeField] float GroundAcceleration = 55f;
        [SerializeField] float GroundDeceleration = 25f;
        [SerializeField] float AirAcceleration = 25f;
        [SerializeField] float AirDeceleration = 1.3f;
        [SerializeField] float UpGravity = 15f;
        [SerializeField] float DownGravity = 25f;
        [Tooltip("�X�^�~�i�̉񕜗�(per 1sec)")] [SerializeField] private int _recoverStamina;
        [Tooltip("�X�^�~�i�̉񕜗�[�X�^�~�i�؂ꎞ](per 1sec)")] [SerializeField] private int _recoverStaminaOnlyTired;
        [Tooltip("�X�^�~�i�̏����(per 1sec)")] [SerializeField] private int _expandStamina;

        bool _isTiredPenalty = false;
        bool _isCanMove = true;
        bool _isCannotMoveByParalyze = false;

        //��ɊO���X�N���v�g�ň����t�B�[���h
        bool _isParalyzed = false;//�g�̖̂��.BodyParalyze.Cs�Ŏg�p
        bool _isPulsation = false;//�S��������.IncreasePulsation.Cs�Ŏg�p


        [Header("�J�����֌W")]
        Vector3 _nowCameraAngle;
        public Vector3 NowCameraAngle { get { return _nowCameraAngle; } }

        public override void Spawned()
        {
            name = $"{Object.InputAuthority} ({(HasInputAuthority ? "Input Authority" : (HasStateAuthority ? "State Authority" : "Proxy"))})";

            if (HasInputAuthority == false)
            {
                //����v���C���[��VirtualCamera���I�t�ɂ���
                var virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>(true);
                for (int i = 0; i < virtualCameras.Length; i++)
                {
                    virtualCameras[i].enabled = false;
                }
            }

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
                    {
                        Debug.Log("�_�b�V��");
                        StartCoroutine(DecreaseStamina());
                    }
                        
                    else if (state != PlayerActionState.Dash)//�X�^�~�i���񕜂ł����Ԃ̎�
                        StartCoroutine(IncreaseStamina());
                }).AddTo(this);

            _myPlayerStatus.OnPlayerStaminaChange
                //.Where(x => x <= 0)
                .Subscribe(_ =>
                {
                    if(_ <= 0)
                    {
                        _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);    //���s��ԂɕύX
                        StartCoroutine(CountTiredPenalty());
                    }
                    
                }).AddTo(this);
            #endregion Subscribes

            StartCoroutine(CheckParalyze());
        }

        public override void FixedUpdateNetwork()
        {
            var input = GetInput<GameplayInput>();
            ProcessInput(input.GetValueOrDefault(), _input.PreviousButtons);
        }

        //���͂̏���
        void ProcessInput(GameplayInput input, NetworkButtons previousButtons)
        {
            _simpleKCC.AddLookRotation(input.LookRotation, -89f, 89f);

            _simpleKCC.SetGravity(_simpleKCC.RealVelocity.y >= 0f ? -UpGravity : -DownGravity);

            
            if (input.MoveDirection == Vector2.zero) //�ړ����͂��������
            {
                _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Idle);
            }
            //�ړ��\��Ԃł����
            else if(_isCanMove && !_isCannotMoveByParalyze && _myPlayerStatus.nowPlayerSurvive && !_myPlayerStatus.nowReviveAnimationDoing)
            {
                //�_�b�V���L�[��������Ă����ԂŔ��Ă��Ȃ����
                if (input.Buttons.IsSet(EInputButton.Dash) && !_isTiredPenalty)
                {
                    //ASD���͂��������
                    if (input.MoveDirection.x == 0 && input.MoveDirection.y > 0)
                    {
                        _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Dash);
                    }
                    else
                        _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);
                }
                //�X�j�[�N�L�[��������Ă����
                else if (input.Buttons.IsSet(EInputButton.Sneak))
                {
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Sneak);
                }
                //����������Ă��Ȃ����
                else
                {
                    _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Walk);
                }
            }
            //�ړ��s�\��Ԃł����
            else
            {
                _myPlayerStatus.ChangePlayerActionState(PlayerActionState.Idle);
            }

            //�v���C���[�̏�Ԃɉ������������x��ݒ�
            _moveAdjustValue = _ajustValueOfState[_myPlayerStatus.nowPlayerActionState];

            var moveDirection = _simpleKCC.TransformRotation * new Vector3(input.MoveDirection.x, 0f, input.MoveDirection.y);
            //Walk,Dash,Sneak�ɉ������ړ����x���v�Z
            var desireMoveVelocity = moveDirection * moveSpeed * _moveAdjustValue;
            MovePlayer(desireMoveVelocity);

            var pitchRotation = _simpleKCC.GetLookRotation(true, false);
            CameraPivot.localRotation = Quaternion.Euler(pitchRotation);

        }

        //���������ֈړ�
        void MovePlayer(Vector3 desiredMoveVelocity = default)
        {
            float accelaration;
            if (desiredMoveVelocity == Vector3.zero)
            {
                accelaration = _simpleKCC.IsGrounded == true ? GroundDeceleration : AirDeceleration;
            }
            else
            {
                accelaration = _simpleKCC.IsGrounded == true ? GroundAcceleration : AirAcceleration;
            }

            _moveVelocity = Vector3.Lerp(_moveVelocity, desiredMoveVelocity, accelaration * Runner.DeltaTime);

            _simpleKCC.Move(_moveVelocity);
        }

        IEnumerator DecreaseStamina()
        {
            while (_myPlayerStatus.nowPlayerActionState == PlayerActionState.Dash)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("�X�^�~�i����");
                _myPlayerStatus.ChangeStamina(_expandStamina / 10 * (_isPulsation ? 2 : 1), ChangeValueMode.Damage);
            }
        }

        IEnumerator IncreaseStamina()
        {
            yield return null;

            if (_isTiredPenalty)//�X�^�~�i���S���
            {
                yield return new WaitForSeconds(0.5f);
                while (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Dash)
                {
                    yield return new WaitForSeconds(0.1f);
                    _myPlayerStatus.ChangeStamina(_recoverStaminaOnlyTired / 10, ChangeValueMode.Heal);
                }
            }
            else//�ʏ펞
            {
                while (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Dash)
                {
                    yield return new WaitForSeconds(0.1f);
                    _myPlayerStatus.ChangeStamina(_recoverStamina / 10, ChangeValueMode.Heal);
                }
            }

        }

        IEnumerator CountTiredPenalty()
        {
            _isTiredPenalty = true;
            yield return new WaitUntil(() => _myPlayerStatus.nowStaminaValue == 100);//�X�^�~�i��100�܂ŉ񕜂���̂�҂�
            _isTiredPenalty = false;
        }

        IEnumerator CheckParalyze()
        {
            while (true)
            {
                yield return new WaitForSeconds(5.0f);
                if (_isParalyzed)
                {
                    //25%�̊m����1�b�ԓ����Ȃ�
                    int random = UnityEngine.Random.Range(0, 4);
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

