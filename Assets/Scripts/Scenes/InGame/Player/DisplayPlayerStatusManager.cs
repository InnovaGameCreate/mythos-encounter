using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

namespace Scenes.Ingame.Player
{ 
    /// <summary>
    /// PlayerStatus�̏������ƂɃQ�[����UI�Ƀv���C���[�S���̏���\��������ׂ̃X�N���v�g
    /// ��قǃl�b�g���[�N�ł́A�����l�̏��̕\���ɑΉ��ł���`��S������
    /// </summary>
    public class DisplayPlayerStatusManager : MonoBehaviour
    {
        [SerializeField] private Slider[] _healthSliders;
        [SerializeField] private Slider[] _sanValueSliders;
        [SerializeField] private TMP_Text[] _healthText;
        [SerializeField] private TMP_Text[] _sanText;

        /// <summary>
        /// Slider�̒l��ς���ׂ̊֐�
        /// </summary>
        /// <param name="value">Slinder.Value�ɑ������l</param>
        /// <param name="ID">�v���C���[ID</param>
        /// <param name="mode">Health(�̗�), SanValue(SAN�l)�ǂ����ύX����̂�������</param>
        public void ChangeSliderValue(int value , int ID, string mode)
        {
            if (mode == "Health")
            {
                _healthSliders[ID].value = value;
                _healthText[ID].text = value.ToString();
            }

            else if (mode == "SanValue")
            { 
                _sanValueSliders[ID].value = value;
                _sanText[ID].text = value.ToString();
            }
                
        }
    }
}

