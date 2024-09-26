using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

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

        private Volume _volume;
        private Vignette _vignette;
        public void Setup()
        {
            _volume = FindObjectOfType<Volume>();
            if (!_volume.profile.TryGet<Vignette>(out _vignette))
            {
                _vignette = _volume.profile.Add<Vignette>(false);
            }
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }

            //���싷�߂�
            _vignette.active = true;//Vignette�̗L����

            //Map�Ƀm�C�Y�𑖂点��
        }

        public void Hide()
        {
            _vignette.active = false;//Vignette�̖�����
        }
    }
}