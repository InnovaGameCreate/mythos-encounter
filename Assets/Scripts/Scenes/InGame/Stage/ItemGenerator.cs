using Scenes.Ingame.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Scenes.Ingame.InGameSystem;
using System.Diagnostics.Tracing;

namespace Scenes.Ingame.Stage
{
    public class ItemGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("脱出アイテムの生成数")]
        private int _escapeItemCount = 4;
        [SerializeField, Tooltip("脱出アイテムの生成数")]
        private int _stageItemCount = 20;
        [SerializeField, Tooltip("itemのprefab")]
        private List<GameObject> _stageItemPrefab;
        [SerializeField, Tooltip("脱出アイテムのprefab")]
        private GameObject _escapeItemPrefab;
        [SerializeField, Tooltip("脱出地点のprefab")]
        private GameObject _escapePointPrefab;
        private List<GameObject> _escapeItems;
        void Start()
        {
            IngameManager.Instance.OnFinishStageGenerateEvent
                .Subscribe(_ =>
                {
                    _escapeItems = GameObject.FindGameObjectsWithTag("ItemSpawnPoint").ToList();
                    Debug.Log($"_escapeItems = {_escapeItems.Count}");
                    InstatiateEscapeItem();
                    RandomStageItemSet();
                    ClearList();
                }).AddTo(this);
        }

        private void InstatiateEscapeItem()
        {
            if(_escapeItems.Count < _escapeItemCount)
            {
                Debug.LogError("escapeMarkerの数が生成するescapeItemの数より少ないです");
                return;
            }
            for (int i = 0; i < _escapeItemCount; i++)
            {
                int randomNumber = Random.Range(0, _escapeItems.Count);
                Instantiate(_escapeItemPrefab, _escapeItems[randomNumber].transform.position, Quaternion.identity);
                DeleteList(_escapeItems[randomNumber]);
            }
        }
        private void RandomStageItemSet()
        {
            if(_escapeItems.Count < 1)
            {
                Debug.LogError("escapeMarkerの数がありません");
                return;
            }
            else if (_escapeItems.Count < _stageItemCount)
            {
                Debug.LogWarning("escapeMarkerの数が少ないため、生成数を調整します");
                _stageItemCount = _escapeItems.Count;
            }
            for (int i = 0; i < _stageItemCount; i++)
            {
                int randomNumber = Random.Range(0, _escapeItems.Count);
                int itemRandomNumber = Random.Range(0, _stageItemPrefab.Count);
                Instantiate(_stageItemPrefab[itemRandomNumber], _escapeItems[randomNumber].transform.position, Quaternion.identity);
                DeleteList(_escapeItems[randomNumber]);
            }
        }
        private void ClearList()
        {
            foreach (var item in _escapeItems)
            {
                DeleteList(item);
            }
        }
        private void DeleteList(GameObject target)
        {
            _escapeItems.Remove(target);
            Destroy(target);
        }
    }
}