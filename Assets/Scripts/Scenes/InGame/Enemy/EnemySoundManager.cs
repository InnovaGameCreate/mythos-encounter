using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Player;

namespace Scenes.Ingame.Enemy
{
    public class EnemySoundManager : MonoBehaviour
    {
        [Header("���g�ɂ��Ă��郁�\�b�h")]
        [SerializeField] private EnemyStatus _enemyStatus;
        [SerializeField] private AudioSource _audioSource;

        [Header("����")]
        [SerializeField] private AudioClip _slowClips;//�����Ƃ��̑�����Clip
        [SerializeField] private AudioClip _fastClips;//����Ƃ��̑�����Clip


        //[Header("���g�ɂ��Ă��郁�\�b�h")]

        // Start is called before the first frame update
        void Start()
        {
            _enemyStatus.OnEnemyStateChange.Subscribe(state =>
            {
                switch (state)
                {
                    case EnemyState.Patrolling:
                        _audioSource.clip = _slowClips;
                        _audioSource.Play();
                        break;
                    case EnemyState.Searching:
                        _audioSource.clip = _slowClips;
                        _audioSource.Play();
                        break;
                    case EnemyState.Chase:
                        _audioSource.clip = _fastClips;
                        _audioSource.Play();
                        break;
                    case EnemyState.Attack:
                        _audioSource.clip = _fastClips;
                        _audioSource.Play();
                        break;
                }


                _audioSource.PlayOneShot(_slowClips);
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
