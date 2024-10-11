using UnityEngine;
using System.Linq;
using Scenes.Ingame.Player;
using UniRx;
using System;
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
        private ReactiveProperty<StageTile> _stageTile = new ReactiveProperty<StageTile>();
        public IObservable<StageTile> OnStageTileChange { get { return _stageTile; } }
        private Vector3 direction = new Vector3(0, -1, 0);

        public void Init()
        {
            _audioSource = GetComponent<AudioSource>();
            _enemy = GameObject.FindWithTag("Enemy");
            _stageInteracts = GameObject.FindGameObjectsWithTag("StageIntract");

            StartCoroutine(FloorCheck());
        }

        IEnumerator FloorCheck()
        {
            LayerMask floorMask = LayerMask.GetMask("Floor");
            RaycastHit hit;
            while (true)
            {
                Ray ray = new Ray(_enemy.transform.position, direction);
                // レイをデバッグ表示
                Debug.DrawRay(ray.origin, ray.direction * 1.0f, Color.red, 2.0f);
                if (Physics.Raycast(ray.origin, ray.direction, out hit, 1.0f, floorMask))
                {
                    _floor = hit.collider.gameObject;
                    _stageTile.Value = _floor.GetComponent<StageTile>();
                }    
                yield return new WaitForSeconds(1f);
            }
        }

        public void Temperature(float change)
        {
            _stageTile.Value.TemperatureChange(change);
            Debug.Log(_stageTile.Value.Temperature);
        }

        public void Msv(int change)
        {
            _stageTile.Value.MsvChange(change);
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