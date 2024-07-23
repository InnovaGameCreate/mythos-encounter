using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �v���C���[�̉e���ŏo�鉹���Ǘ�����B
    /// ����,�A�C�e���̉��Ȃ�
    /// </summary>
    public class PlayerSoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioClip[] _footClips;//������Clip
        [SerializeField] private AudioClip[] _screamClips;//�ߖ�Clip
        [SerializeField] private AudioClip[] _itemClips;//�A�C�e����Clip
        [SerializeField] private AudioClip[] _effectClips;//���ʉ���Clip

        private PlayerStatus _myPlayerStatus;
        // Start is called before the first frame update
        void Start()
        {
            TryGetComponent<PlayerStatus>(out _myPlayerStatus);
        }

        public void StopSound()
        {
            _audio.loop = false;
            _audio.Stop();
        }


        /// <summary>
        /// Scream.Cs�ɂāA�l�����Ԏ��̌��ʉ��𔭐�������֐��B
        /// </summary>
        /// <param name="gender">����.�j�FMale , �����FFemale</param>
        public void SetScreamClip(string gender)
        {
            _audio.volume = 0.5f;

            if (gender == "Male")
            {
                _audio.PlayOneShot(_screamClips[0]);
            }
            else if(gender == "Female")
            {
                _audio.PlayOneShot(_screamClips[1]);
            }
        }

        /*
         �v���C���[�̑����i�A�j���[�V�����C�x���g�j
         */
        private void SneakingFootSound()
        {
            if (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Sneak)
                return;

            _audio.volume = 0.1f;
            _audio.PlayOneShot(_footClips[0]);

        }

        private void WalkingFootSound(int order)
        {
            if (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Walk)
                return;

            _audio.volume = 0.5f;
            //�A�j���[�V�������̃C�x���g�̏��Ԃɉ����đ�����ς���
            //�������E��
            if (order == 0)//��
            {
                _audio.PlayOneShot(_footClips[1]);
            }
            else//�E
            {
                _audio.PlayOneShot(_footClips[2]);
            }
        }

        private void RunningFootSound(int order)
        {
            if (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Dash)
                return;

            _audio.volume = 0.75f;
            //�A�j���[�V�������̃C�x���g�̏��Ԃɉ����đ�����ς���
            //�������E��
            if (order == 0)//��
            {
                _audio.PlayOneShot(_footClips[3]);
            }
            else//�E
            {
                _audio.PlayOneShot(_footClips[4]);
            }
        }

    }
}

