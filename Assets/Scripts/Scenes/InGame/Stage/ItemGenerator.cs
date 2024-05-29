using Scenes.Ingame.Manager;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

namespace Scenes.Ingame.Stage
{
    public class ItemGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("�E�o�A�C�e���̐�����")]
        private int _escapeItemCount = 4;
        [SerializeField, Tooltip("�X�e�[�W�A�C�e���̐�����")]
        private int _stageItemCount = 20;
        [SerializeField, Tooltip("item��prefab")]
        private List<GameObject> _stageItemPrefab;
        [SerializeField, Tooltip("�E�o�A�C�e����prefab")]
        private GameObject _escapeItemPrefab;
        [SerializeField, Tooltip("�E�o�n�_��prefab")]
        private GameObject _escapePointPrefab;
        private List<GameObject> _itemMarker;
        void Start()
        {
            IngameManager.Instance.OnStageGenerateEvent
                .Subscribe(_ =>
                {
                    _itemMarker = GameObject.FindGameObjectsWithTag("ItemSpawnPoint").ToList();
                    InstatiateEscapeItem();
                    RandomStageItemSet();
                    ClearList();
                }).AddTo(this);
        }

        private void InstatiateEscapeItem()
        {
            if(_itemMarker.Count < _escapeItemCount)
            {
                Debug.LogError("escapeMarker�̐�����������escapeItem�̐���菭�Ȃ��ł�");
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
                Debug.LogError("escapeMarker�̐�������܂���");
                return;
            }
            else if (_itemMarker.Count < _stageItemCount)
            {
                Debug.LogWarning("escapeMarker�̐������Ȃ����߁A�������𒲐����܂�");
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
            List<GameObject> _clearList = _itemMarker;
            foreach (GameObject item in _clearList)
            {
                Destroy(item);
            }
            _itemMarker.Clear();
        }
        private void DeleteList(GameObject target)
        {
            _itemMarker.Remove(target);
            Destroy(target);
        }
    }
}