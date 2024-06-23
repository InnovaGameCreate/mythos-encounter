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
