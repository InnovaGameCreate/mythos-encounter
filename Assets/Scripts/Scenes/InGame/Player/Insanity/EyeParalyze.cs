using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �����̎�ނ�1��
    /// 1.�~�j�}�b�v�ɃO���b�`�m�C�Y�����茩���Ȃ��Ȃ�
    /// 2.���삪�����Ȃ�iPostProcessing�j
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