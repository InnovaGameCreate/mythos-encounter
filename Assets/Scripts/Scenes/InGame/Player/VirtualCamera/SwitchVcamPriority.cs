using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UniRx;
using Scenes.Ingame.Player;


namespace Scenes.Ingame.Player
{
    public class SwitchVcamPriority : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> _vcamList;
        [SerializeField] private PlayerStatus _myPlayerStatus;
        [SerializeField] private BoolReactiveProperty _isNoised = new BoolReactiveProperty(true);

        private void Start()
        {
            _myPlayerStatus.OnPlayerActionStateChange
                .Skip(1)//����i�X�|�[������j�͍s��Ȃ�
                .Where(state => state == PlayerActionState.Idle || state == PlayerActionState.Walk || state == PlayerActionState.Dash || state == PlayerActionState.Sneak)
                .Subscribe(state =>
                {
                    //���ׂĂ�Priority�����Z�b�g
                    foreach(var _vcam in _vcamList) {
                        _vcam.Priority = 0;
                    }

                    //PlayerActionState���Ƃ�vcam��Priority��ύX����
                    switch (state) {
                        case PlayerActionState.Idle:
                            _vcamList[0].Priority = 10;
                            break;
                        case PlayerActionState.Walk:
                            _vcamList[1].Priority = 10;
                            break;
                        case PlayerActionState.Dash:
                            _vcamList[2].Priority = 10;
                            break;
                        case PlayerActionState.Sneak:
                            _vcamList[3].Priority = 10;
                            break;
                    }

                });

            _isNoised
                .Skip(1)
                .Where(x => x = false)
                .Subscribe(_ =>
                {

                });

        }

    }
}
