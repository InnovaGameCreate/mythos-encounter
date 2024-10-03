using UnityEngine;

using Fusion;
using Fusion.Addons.SimpleKCC;

public enum EInputButton
{
    UseItem,
    GetItem,
    Sneak,
    Dash
}

public struct GameplayInput : INetworkInput
{
    public Vector2 LookRotation;
    public float MouseWheelValue;
    public Vector2 MoveDirection;
    public NetworkButtons Buttons;
}

[DefaultExecutionOrder(-10)]
public sealed class PlayerInput : NetworkBehaviour, IBeforeUpdate, IAfterTick
{
    public NetworkButtons PreviousButtons { get; set; }
    public Vector2 LookRotation => _input.LookRotation;

    static float LookSensitivity = 30f;

    GameplayInput _input;
    Vector2Accumulator _lookRotationAccumulator = new Vector2Accumulator(0.02f, true);

    

    public override void Spawned()
    {
        //���͌����������Ă��Ȃ���Ώ������Ȃ�
        if (HasInputAuthority == false)
            return;

        //�R�[���o�b�N���w��
        var networkEvents = Runner.GetComponent<NetworkEvents>();
        networkEvents.OnInput.AddListener(OnInput);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (runner == null)
            return;

        var networkEvents = runner.GetComponent<NetworkEvents>();
        if (networkEvents != null)
            networkEvents.OnInput.RemoveListener(OnInput);
    }

    void IBeforeUpdate.BeforeUpdate()
    {
        if (HasInputAuthority == false)
            return;

        //���͂��擾
        var lookRotationDelta = new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        //���x��K��
        lookRotationDelta *= LookSensitivity / 60f;
        //���͂�~��
        _lookRotationAccumulator.Accumulate(lookRotationDelta);

        //�z�C�[���̓��͂��擾
        _input.MouseWheelValue += Input.GetAxis("Mouse ScrollWheel");
        

        var moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _input.MoveDirection = moveDirection.normalized;
        

        _input.Buttons.Set(EInputButton.Sneak, Input.GetKey(KeyCode.LeftControl));
        _input.Buttons.Set(EInputButton.Dash, Input.GetKey(KeyCode.LeftShift));
    }

    void IAfterTick.AfterTick()
    {
        PreviousButtons = GetInput<GameplayInput>().GetValueOrDefault().Buttons;
    }

    void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
        _input.LookRotation = _lookRotationAccumulator.ConsumeTickAligned(runner);
        networkInput.Set(_input);
    }
}

