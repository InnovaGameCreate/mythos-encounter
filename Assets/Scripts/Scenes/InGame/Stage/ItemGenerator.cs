using Scenes.Ingame.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Scenes.Ingame.InGameSystem;
using System.Diagnostics.Tracing;
using static UnityEditor.Progress;

namespace Scenes.Ingame.Stage
{
    public class ItemGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("脱出アイテムの生成数")]
        private int _escapeItemCount = 4;
        [SerializeField, Tooltip("ステージアイテムの生成数")]
        private int _stageItemCount = 20;
        [SerializeField, Tooltip("itemのprefab")]
        private List<GameObject> _stageItemPrefab;
        [SerializeField, Tooltip("脱出アイテムのprefab")]
        private GameObject _escapeItemPrefab;
        [SerializeField, Tooltip("脱出地点のprefab")]
        private GameObject _escapePointPrefab;
        private List<GameObject> _itemMarker;
        void Start()
        {
            IngameManager.Instance.OnFinishStageGenerateEvent
                .Subscribe(_ =>
                {
                    _itemMarker = GameObject.FindGameObjectsWithTag("ItemSpawnPoint").ToList();
                    Debug.Log($"_escapeItems = {_itemMarker.Count}");
                    InstatiateEscapeItem();
                    RandomStageItemSet();
                    ClearList();
                }).AddTo(this);
        }

        private void InstatiateEscapeItem()
        {
            if(_itemMarker.Count < _escapeItemCount)
            {
                Debug.LogError("escapeMarkerの数が生成するescapeItemの数より少ないです");
                return;
            }
            for (int i = 0; i < _escapeItemCount; i++)
            {
                int randomNumber = Random.Range(0, _itemMarker.Count);
                Instantiate(_escapeItemPrefab, _itemMarker[randomNumber].transform.position, Quaternion.identity);
                DeleteList(_itemMarker[randomNumber]);
            }
        }
        private void RandomStageItemSet()
        {
            if(_itemMarker.Count < 1)
            {
                Debug.LogError("escapeMarkerの数がありません");
                return;
            }
            else if (_itemMarker.Count < _stageItemCount)
            {
                Debug.LogWarning("escapeMarkerの数が少ないため、生成数を調整します");
                _stageItemCount = _itemMarker.Count;
            }
            for (int i = 0; i < _stageItemCount; i++)
            {
                int randomNumber = Random.Range(0, _itemMarker.Count);
                int itemRandomNumber = Random.Range(0, _stageItemPrefab.Count);
                Instantiate(_stageItemPrefab[itemRandomNumber], _itemMarker[randomNumber].transform.position, Quaternion.identity);
                DeleteList(_itemMarker[randomNumber]);
            }
        }
        private void ClearList()
        {
            for (int i = 0; i < _itemMarker.Count; i++)
            {
                DeleteList(_itemMarker[i]);
            }
        }
        private void DeleteList(GameObject target)
        {
            _itemMarker.Remove(target);
            Destroy(target);
        }
    }
}