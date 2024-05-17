using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// プレイヤーの影響で出る音を管理する。
    /// 足音,アイテムの音など
    /// </summary>
    public class PlayerSoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioClip[] _footClips;//足音のClip
        [SerializeField] private AudioClip[] _screamClips;//悲鳴のClip
        [SerializeField] private AudioClip[] _itemClips;//アイテムのClip
        [SerializeField] private AudioClip[] _effectClips;//効果音のClip

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
        /// 現在設定しているClipの長さ（秒数）を取得する関数
        /// </summary>
        /// <returns></returns>
        public float GetClipLength()
        {
            //足が動く速さが変化する状態の時
            if (_myPlayerStatus.nowPlayerActionState == PlayerActionState.Walk || _myPlayerStatus.nowPlayerActionState == PlayerActionState.Dash ||
                _myPlayerStatus.nowPlayerActionState == PlayerActionState.Sneak)
            {
                return _audio.clip.length;
            }
            else
                return 0;
        }
        /// <summary>
        /// 足音を鳴らすための関数
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
        /// Scream.Csにて、人が叫ぶ時の効果音を発生させる関数。
        /// </summary>
        /// <param name="gender">性別.男：Male , 女性：Female</param>
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

