using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Scenes.Ingame.Player;


public class VcamExtensionSneak : VcamExtension 
{
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        base.PostPipelineStageCallback(vcam, stage, ref state, deltaTime);
        if( VcamExtension.Player != null) {
            if (stage == CinemachineCore.Stage.Finalize) {
                if (PlayerStatus.nowPlayerActionState == PlayerActionState.Sneak) {
                    vcam.m_Priority = 10;
                }
                else {
                    vcam.m_Priority = 0;
                }
            }
        }
    }

}
