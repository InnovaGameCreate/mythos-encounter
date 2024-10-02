using UnityEngine;
using System.Linq;
using Scenes.Ingame.Player;
using UniRx;
using System;
using static UnityEngine.UI.Image;
using Scenes.Ingame.Stage;
using System.Collections;

namespace Scenes.Ingame.Enemy.Trace.Feature
{
    public class FeatureView : MonoBehaviour
    {
        GameObject _enemy;//TODO:敵が複数体だった場合の対応
        private const float RANGE = 5;
        private GameObject[] _stageInteracts;
        private GameObject[] nearStageObject = null;
        private GameObject interactTarget = null;
        private GameObject _floor;
        AudioSource _audioSource;
        private Subject<Unit> _onDestroy = new Subject<Unit>();
        public IObservable<Unit> onDestroy { get => _onDestroy; }
        public ReactiveProperty<GameObject> TemperatureTrace = new ReactiveProperty<GameObject>();
        private Vector3 direction = new Vector3(0, -1, 0);
        public StageTile stagetile;

        public void Init()
        {
            _audioSource = GetComponent<AudioSource>();
            _enemy = GameObject.FindWithTag("Enemy");
            _stageInteracts = GameObject.FindGameObjectsWithTag("StageIntract");
            StartCoroutine(FloorCheck());
        }

        IEnumerator FloorCheck()
        {
            Ray ray = new Ray(_enemy.transform.position, direction);
            RaycastHit hit;
            while (true)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.layerOverridePriority == 7)
                    {
                        _floor = hit.collider.gameObject;
                        stagetile = _floor.GetComponent<StageTile>();
                        TemperatureTrace.Value = _floor;
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public void Temperature(float change)
        {
            stagetile.TemperatureChange(change);
        }

        public void Msv(int change)
        {
            stagetile.MsvChange(change);
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

        private void OnDestroy()
        {
            _onDestroy.OnNext(default);
        }
    }
}