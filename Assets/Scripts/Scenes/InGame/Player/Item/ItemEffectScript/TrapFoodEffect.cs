using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UIElements;

namespace Scenes.Ingame.Player
{
    public class TrapFoodEffect : ItemEffect
    {
        private readonly float _tileLength = 5.85f;//�P�^�C���̒���
        private GameObject _mainCamera;
        private GameObject _trapFoodPrefab;
        private bool _isCanCreate = false;
        private bool _isCanLoopCoroutine;
        [SerializeField] private GameObject _createdTrapFood;


        public override void OnPickUp()
        {
            _isCanLoopCoroutine = true;
            StartCoroutine(CreateTrapBite());

            //�I���A�C�e����ʂ̂��̂ɂ����Ƃ��A�����Ńv���r���[���폜����
            ownerPlayerItem.OnNowIndexChange
                .Skip(1)
                //.Where(x => ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData == null || ownerPlayerItem.ItemSlots[ownerPlayerItem.nowIndex].myItemData.itemID != 21)
                .Subscribe(_ =>
                {
                    StopCoroutine(CreateTrapBite());
                    if (_createdTrapFood != null)
                    {
                        Destroy(_createdTrapFood);
                    }

                }).AddTo(this);
        }

        public override void OnThrow()
        {
            _isCanLoopCoroutine = false;
        }

        public override void Effect()
        {
            if (_isCanCreate && _createdTrapFood != null)
            {
                _createdTrapFood.transform.Find("EnemySensor").gameObject.SetActive(true);
                _createdTrapFood.layer = 10;
                StopCoroutine(CreateTrapBite());
                ownerPlayerItem.ConsumeItem(ownerPlayerItem.nowIndex);
            }
        }

        private IEnumerator CreateTrapBite()
        {
            _mainCamera = GameObject.FindWithTag("MainCamera");
            _trapFoodPrefab = (GameObject)Resources.Load("Prefab/Item/TrapFood/UsingTrapFood");
            RaycastHit hit;
            int floorlayerMask = LayerMask.GetMask("Floor");

            while (true)
            {
                yield return null;

                Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _tileLength, floorlayerMask);
                Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * _tileLength, Color.black);

                if (hit.collider != null)
                {
                    //�v���r���[�̍쐬�E�ړ�
                    if (_createdTrapFood == null)//�v���r���[���Ȃ��Ƃ��͍쐬
                    {
                        _createdTrapFood = Instantiate(_trapFoodPrefab, hit.point, _trapFoodPrefab.transform.rotation);
                    }
                    else
                    {
                        _createdTrapFood.transform.position = hit.point;
                    }

                    if (_createdTrapFood.GetComponent<TrapFoodCheckCollider>().IsTriggered && _isCanCreate)
                    {
                        _isCanCreate = false;
                    }
                    else if (!_createdTrapFood.GetComponent<TrapFoodCheckCollider>().IsTriggered && !_isCanCreate)
                    {
                        _isCanCreate = true;
                    }
                }
                else
                {
                    _isCanCreate = false;
                    if (_createdTrapFood != null)//�v���r���[���쐬����Ă�����j��
                    {
                        Destroy(_createdTrapFood);
                    }
                }

            }

        }



    }
}
