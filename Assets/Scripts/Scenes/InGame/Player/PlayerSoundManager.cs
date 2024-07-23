using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            TryGetComponent<PlayerStatus>(out _myPlayerStatus);
        }

        public void StopSound()
        {
            _audio.loop = false;
            _audio.Stop();
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

        /*
         プレイヤーの足音（アニメーションイベント）
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
            //アニメーション中のイベントの順番に応じて足音を変える
            //左足→右足
            if (order == 0)//左
            {
                _audio.PlayOneShot(_footClips[1]);
            }
            else//右
            {
                _audio.PlayOneShot(_footClips[2]);
            }
        }

        private void RunningFootSound(int order)
        {
            if (_myPlayerStatus.nowPlayerActionState != PlayerActionState.Dash)
                return;

            _audio.volume = 0.75f;
            //アニメーション中のイベントの順番に応じて足音を変える
            //左足→右足
            if (order == 0)//左
            {
                _audio.PlayOneShot(_footClips[3]);
            }
            else//右
            {
                _audio.PlayOneShot(_footClips[4]);
            }
        }

    }
}

