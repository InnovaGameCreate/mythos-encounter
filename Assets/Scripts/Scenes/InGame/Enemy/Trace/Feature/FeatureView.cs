using UnityEngine;
using System.Linq;
using Scenes.Ingame.Player;

namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class FeatureView : MonoBehaviour
    {
        GameObject _enemy;//TODO:敵が複数体だった場合の対応
        private const float RANGE = 5;
        private GameObject[] _stageInteracts;
        private GameObject[] nearStageObject = null;
        private GameObject interactTarget = null;
        AudioSource _audioSource;

        public void Init(AudioSource audioSource)
        {
            _audioSource = audioSource;
            _enemy = GameObject.FindWithTag("Enemy");
            _stageInteracts = GameObject.FindGameObjectsWithTag("StageIntract");
        }
        
        public void Breath()
        {
            _audioSource.transform.position = _enemy.transform.position;
            _audioSource.Play();
        }

        public void TryInteract()
        {
            nearStageObject = _stageInteracts.Where(target => Vector3.Distance(target.transform.position, _enemy.transform.position) < RANGE).ToArray();
            if (nearStageObject.Length > 0)
            {
                interactTarget = nearStageObject[UnityEngine.Random.Range(0, nearStageObject.Length)];
                if (interactTarget.TryGetComponent(out IInteractable act))
                {
                    act.Intract(null, true);
                }
            }
        }
    }
}