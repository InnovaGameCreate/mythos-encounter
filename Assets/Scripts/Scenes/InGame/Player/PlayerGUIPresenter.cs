using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// PlayerStatus��DisplayPlayerStatusManager�̋��n�����s���N���X
    /// MV(R)P�ɂ�����Presenter�̖�����z��
    /// </summary>
    public class PlayerGUIPresenter : MonoBehaviour
    {
        //model
        [SerializeField] private PlayerStatus[] _playerStatuses;
        //View
        [SerializeField] private DisplayPlayerStatusManager _displayPlayerStatusManager;


        // Start is called before the first frame update
        void Awake()
        {
            _displayPlayerStatusManager.OnCompleteSort
                .Subscribe(_ => 
                {
                    //DisplayPlayerStatusManager�ɂ���\�[�g�ς݂̔z����擾
                    _playerStatuses = _displayPlayerStatusManager.PlayerStatuses;

                    //�e�v���C���[��HP��SAN�l���ύX���ꂽ�Ƃ��̏�����ǉ�����B
                    foreach (var playerStatus in _playerStatuses)
                    {
                        playerStatus.OnPlayerHealthChange
                            .Subscribe(x =>
                            {
                                //view�ɔ��f
                                _displayPlayerStatusManager.ChangeSliderValue(x, playerStatus.playerID, "Health");
                            }).AddTo(this);

                        playerStatus.OnPlayerSanValueChange
                            .Subscribe(x =>
                            {
                                //view�ɔ��f
                                _displayPlayerStatusManager.ChangeSliderValue(x, playerStatus.playerID, "SanValue");
                            }).AddTo(this);
                    }
                }).AddTo(this);
        }
    }
}
