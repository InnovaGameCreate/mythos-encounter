using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Scenes.Ingame.Player;


public class CinemachineExtensionsDash : CinemachineExtension 
{
    private GameObject _player;
    TempPlayerMove _tempPlayerMove;
    PlayerStatus _playerStatus;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _tempPlayerMove = _player.GetComponent<TempPlayerMove>();
        _playerStatus = _player.GetComponent<PlayerStatus>();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (_player != null)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                var eulerAngles = state.RawOrientation.eulerAngles;
                eulerAngles.x = _tempPlayerMove.NowCameraAngle.x;
                state.RawOrientation = Quaternion.Euler(eulerAngles);
            }
            else if (stage == CinemachineCore.Stage.Finalize)
            {

                if (_playerStatus.nowPlayerActionState == PlayerActionState.Dash)
                {
                    vcam.m_Priority = 10;

                }
                else
                {
                    vcam.m_Priority = 0;
                }
            }
 
        }
        

    }

}
