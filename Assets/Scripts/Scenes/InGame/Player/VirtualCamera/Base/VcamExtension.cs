using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Scenes.Ingame.Player;

/// <summary>
/// VirtualCamera�̎����v�Z�ɂ��J�����̊p�x�E�ʒu�̒����ɂ��ăR�}���h�ŕύX���邽�߂̃N���X
/// </summary>
public class VcamExtension : CinemachineExtension
{
    [SerializeField] private TempPlayerMove _tempPlayerMove;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (_tempPlayerMove != null) {
            if (stage == CinemachineCore.Stage.Aim) {        
                var eulerAngles = state.RawOrientation.eulerAngles;
                eulerAngles.x = _tempPlayerMove.NowCameraAngle.x;
                state.RawOrientation = Quaternion.Euler(eulerAngles);
            }
        }        
    }
}
