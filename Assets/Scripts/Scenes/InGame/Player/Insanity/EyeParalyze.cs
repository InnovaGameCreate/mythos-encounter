using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �����̎�ނ�1��
    /// 1.�~�j�}�b�v�ɃO���b�`�m�C�Y�����茩���Ȃ��Ȃ�
    /// 2.�v���C���[�̉�ʂɂ͊O���ɐԂ������𔭐�������(�o�����Ə������ɂ͂������t�F�[�h����)
    /// 3.�ꕔ��Ⴢ�������
    /// </summary>
    public class EyeParalyze : MonoBehaviour, IInsanity
    {
        private bool _isFirst = true;//���߂ČĂяo���ꂽ��

        public void Setup()
        { 
        
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }
        }

        public void Hide()
        {
            
        }
    }
}