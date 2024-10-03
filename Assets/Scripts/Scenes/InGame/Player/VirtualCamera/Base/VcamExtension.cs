using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Scenes.Ingame.Player;

/// <summary>
/// VirtualCameraの自動計算によるカメラの角度・位置の調整についてコマンドで変更するためのクラス
/// </summary>
public class VcamExtension : CinemachineExtension
{
    [SerializeField] private PlayerMove _playerMove;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (_playerMove != null) {
            if (stage == CinemachineCore.Stage.Aim) {        
                var eulerAngles = state.RawOrientation.eulerAngles;
                eulerAngles.x = _playerMove.NowCameraAngle.x;
                state.RawOrientation = Quaternion.Euler(eulerAngles);
            }
        }        
    }
}
