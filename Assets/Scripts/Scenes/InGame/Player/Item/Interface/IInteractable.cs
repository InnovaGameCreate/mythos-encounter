using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �h�A,�A�C�e���Ȃǂ̉E�N���b�N�ŃA�N�V�������s���邱�Ƃ������C���^�t�F�[�X
    /// </summary>
    public interface IInteractable
    {
        //��邱��
        void Intract(PlayerStatus status);
    }
}
