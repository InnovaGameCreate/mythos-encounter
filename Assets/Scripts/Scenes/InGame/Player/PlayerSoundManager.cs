using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
            _myPlayerStatus = GetComponent<PlayerStatus>();
        }

        public void StopSound()
        {
            _audio.loop = false;
            _audio.Stop();
        }


        /// <summary>
        /// ���ݐݒ肵�Ă���Clip�̒����i�b���j���擾����֐�
        /// </summary>
        /// <returns></returns>
        public float GetClipLength()
        {
            //���������������ω������Ԃ̎�
            if (_myPlayerStatus.nowPlayerActionState == PlayerActionState.Walk || _myPlayerStatus.nowPlayerActionState == PlayerActionState.Dash ||
                _myPlayerStatus.nowPlayerActionState == PlayerActionState.Sneak)
            {
                return _audio.clip.length;
            }
            else
                return 0;
        }
        /// <summary>
        /// ������炷���߂̊֐�
        /// </summary>
        /// <param name="state"></param>
        public void FootSound(PlayerActionState state)
        {
            _audio.loop = true;
            switch (state)
            {
                case PlayerActionState.Idle:
                    _audio.loop = false;
                    _audio.DOFade(endValue: 0f, duration: 0.2f);
                    break;
                case PlayerActionState.Walk:
                    _audio.DOKill();
                    _audio.volume = 0.5f;
                    _audio.clip = _footClips[0];
                    _audio.Play();
                    break;
                case PlayerActionState.Dash:
                    _audio.DOKill();
                    _audio.volume = 0.75f;
                    _audio.clip = _footClips[1];
                    _audio.Play();
                    break;
                case PlayerActionState.Sneak:
                    _audio.DOKill();
                    _audio.volume = 0.1f;
                    _audio.clip = _footClips[2];
                    _audio.Play();
                    break;
            }            
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
    }
}

