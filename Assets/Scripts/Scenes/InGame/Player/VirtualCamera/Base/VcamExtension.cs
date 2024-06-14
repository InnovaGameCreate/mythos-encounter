using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Scenes.Ingame.Player;

public class VcamExtension : CinemachineExtension 
{
    static private GameObject _player;   
    static private TempPlayerMove _tempPlayerMove;
    static private PlayerStatus _playerStatus;
    static public GameObject Player { get { return _player; } }
    static public PlayerStatus PlayerStatus { get {  return _playerStatus; } }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _tempPlayerMove = _player.GetComponent<TempPlayerMove>();
        _playerStatus = _player.GetComponent<PlayerStatus>();
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
        if (_player != null) {
            if (stage == CinemachineCore.Stage.Aim) {        
                var eulerAngles = state.RawOrientation.eulerAngles;
                eulerAngles.x = _tempPlayerMove.NowCameraAngle.x;
                state.RawOrientation = Quaternion.Euler(eulerAngles);
            }
        }
    }
}
