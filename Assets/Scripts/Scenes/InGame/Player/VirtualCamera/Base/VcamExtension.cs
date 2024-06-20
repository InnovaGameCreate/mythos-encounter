using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Scenes.Ingame.Player;

public class VcamExtension : CinemachineExtension
{
    [SerializeField] private TempPlayerMove _tempPlayerMove;
    [SerializeField] private PlayerStatus _playerStatus;
    private CinemachineVirtualCamera _myCinemachineVitualCamera;
    bool _isNoised = true;

    private void Start()
    {
        _myCinemachineVitualCamera = GetComponent<CinemachineVirtualCamera>();
    }

        

    /// <summary>
    /// ヴァーチャルカメラの自動計算処理の中に自分の好きなコマンドを追加できるメソッド
    /// 
    /// </summary>
    /// <param name="vcam"></param>
    /// <param name="stage"></param>
    /// <param name="state"></param>
    /// <param name="deltaTime"></param>
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (_playerStatus != null) {
            if (stage == CinemachineCore.Stage.Aim) {        
                var eulerAngles = state.RawOrientation.eulerAngles;
                eulerAngles.x = _tempPlayerMove.NowCameraAngle.x;
                state.RawOrientation = Quaternion.Euler(eulerAngles);
            }

            if(stage == CinemachineCore.Stage.Noise) {
                if (Input.GetKey(KeyCode.L)){
                    _myCinemachineVitualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
                }
                
            }
        }

        
    }
}
