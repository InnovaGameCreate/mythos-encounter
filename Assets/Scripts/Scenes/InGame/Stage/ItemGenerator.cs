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
        [SerializeField, Tooltip("�E�o�A�C�e���̐�����")]
        private int _escapeItemCount = 4;
        [SerializeField, Tooltip("�E�o�A�C�e���̐�����")]
        private int _stageItemCount = 20;
        [SerializeField, Tooltip("item��prefab")]
        private List<GameObject> _stageItemPrefab;
        [SerializeField, Tooltip("�E�o�A�C�e����prefab")]
        private GameObject _escapeItemPrefab;
        [SerializeField, Tooltip("�E�o�n�_��prefab")]
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
                Debug.LogError("escapeMarker�̐�����������escapeItem�̐���菭�Ȃ��ł�");
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
                Debug.LogError("escapeMarker�̐�������܂���");
                return;
            }
            else if (_escapeItems.Count < _stageItemCount)
            {
                Debug.LogWarning("escapeMarker�̐������Ȃ����߁A�������𒲐����܂�");
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