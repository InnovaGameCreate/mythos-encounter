using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �����̐��`�ƂȂ钊�ۃN���X.
    /// �q�N���X�̖��O�́Z�ZMagic�Ƃ��邱��
    /// �e�q�N���X�͒���player�ɃA�^�b�`����\��.
    /// </summary>
    public abstract class Magic : MonoBehaviour
    {
        [HideInInspector] public float coolTime;
        [HideInInspector] public int consumeSanValue;

        public void Start()
        {
            ChangeFieldValue();
        }
        /// <summary>
        /// �����̌��ʂ���������֐�
        /// </summary>
        public abstract void MagicEffect();

        /// <summary>
        /// �ϐ���ݒ肷��֐�
        /// </summary>
        public abstract void ChangeFieldValue();
    }
}
