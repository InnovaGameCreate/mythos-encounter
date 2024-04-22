using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �A�C�e���Ɋւ��鏈�����܂Ƃ߂��N���X
    /// 1.�A�C�e���X���b�g�ɂ���A�C�e�����g�p����
    /// 2.�����A�C�e���̊Ǘ�
    /// 3.�A�C�e���X���b�g�̈ʒu�̊Ǘ�
    /// </summary>
    public class PlayerItem : MonoBehaviour
    {
        private PlayerStatus _myPlayerStatus;

        //�A�C�e���֌W
        private ReactiveProperty<int> _nowIndex = new ReactiveProperty<int>();//�I�𒆂̃A�C�e���X���b�g�ԍ�
        
        //Ray�֘A
        [SerializeField] Camera _mainCamera;//player�̖ڐ���S���J����
        [SerializeField] private float _getItemRange = 2.0f;//�A�C�e�������ł��鋗��

        //�A�C�e���X���b�g�iUI�j�̑���֘A
        private float scrollValue;
        [SerializeField] private float scrollSense = 10;//�}�E�X�z�C�[���̊��x

        private bool _isCanUseItem = true;

        //UniRx�֌W
        private Subject<String> _itemPopActive = new Subject<String>();
        private ReactiveCollection<ItemSlotStruct> _itemSlot = new ReactiveCollection<ItemSlotStruct>();//���ݏ������Ă���A�C�e���̃��X�g

        public List<ItemSlotStruct> ItemSlots { get { return _itemSlot.ToList(); } }//�O����_itemSlot�̓��e�����J����
        public int nowIndex { get => _nowIndex.Value; }


        public IObservable<int> OnNowIndexChange { get { return _nowIndex; } }//�O����_nowIndex�̒l���ύX���ꂽ�Ƃ��ɍs��������o�^�ł���悤�ɂ���
        public IObservable<String> OnItemPopActive { get { return _itemPopActive; } }
        public IObservable<CollectionReplaceEvent<ItemSlotStruct>> OnItemSlotReplace => _itemSlot.ObserveReplace();//�O����_itemSlot�̗v�f���ύX���ꂽ�Ƃ��ɍs��������o�^�ł���悤�ɂ���


        // Start is called before the first frame update
        void Start()
        {
            int layerMask = LayerMask.GetMask("Item");//Item�Ƃ������C���[�ɂ���GameObject�ɂ���ray��������Ȃ��悤�ɂ���
            _myPlayerStatus = GetComponent<PlayerStatus>();

            //�����ingame�O�̃A�C�e���̏����󋵂���������B���ł͏�����
            ItemSlotStruct init = new ItemSlotStruct();            
            for (int i = 0; i < 7; i++)
            {
                init.ChangeInfo(null, ItemSlotStatus.available);
                _itemSlot.Add(init);
            }

            //�����̐�ɃA�C�e�������邩�m�F�B����ΉE�N���b�N�ŏE���ł���悤�ɂ���
            this.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _getItemRange, layerMask))//�ݒ肵�������ɂ���A�C�e����F�m
                        {
                            _itemPopActive.OnNext(hit.collider.name);//�A�C�e���|�b�v���o��
                            //TryGetComponent���s���B
                            if (hit.collider.gameObject.TryGetComponent(out IInteractable intract))
                            {
                                intract.Intract(_myPlayerStatus);
                            }

                        }
                        else
                        {
                            _itemPopActive.OnNext(null);//�A�C�e���|�b�v���A�N�e�B�u��
                        }
                    });

            //���N���b�N�����Ƃ��ɃA�C�e�����g�p
            this.UpdateAsObservable()
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetMouseButtonDown(0) && _isCanUseItem)
                    .Subscribe(_ =>
                    {
                        Debug.Log("�A�C�e���g��");

                        //�A�C�e�����g�p
                        _itemSlot[_nowIndex.Value].myItemData.thisItemEffect.Effect();

                        //�����g���؂�A�C�e���ł����List��,�Ή����鏇�Ԃ�����������
                        if (_itemSlot[_nowIndex.Value].myItemData.isSingleUse)
                        {
                            ThrowItem(_nowIndex.Value);
                        }
                    });

            //H�L�[����͂����Ƃ��ɃA�C�e����j��            
            this.UpdateAsObservable()
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetKeyDown(KeyCode.H))
                    .Subscribe(_ =>
                    {
                        //�̂Ă��A�C�e�����߂��ɕ������A�ēx�E����悤�ɂ���B
                        //�����i�������j

                        //�A�C�e�����̂Ă�B
                        ThrowItem(_nowIndex.Value);
                    });

            //�v���C���[�̓��͂ɂ��_nowIndex�̕ύX
            //1.�}�E�X�z�C�[���̓���
            //2.�����L�[�̓���
            this.UpdateAsObservable()
                    .Where(_ => Input.GetAxis("Mouse ScrollWheel") != 0 || ItemNumberKeyDown() != 0)
                    .Subscribe(_ =>
                    {
                        //�}�E�X�z�[���݂̂̓��͎�
                        if (ItemNumberKeyDown() == 0)
                        {
                            scrollValue += Input.GetAxis("Mouse ScrollWheel") * scrollSense;
                            scrollValue = Mathf.Clamp(scrollValue,0,6);
                            _nowIndex.Value = (int)scrollValue;
                        }
                        //�����L�[�݂̂̓��͎�
                        if (Input.GetAxis("Mouse ScrollWheel") == 0)
                        {
                            _nowIndex.Value = ItemNumberKeyDown() - 49;
                            scrollValue = _nowIndex.Value;
                        }                           
                    });
        }

        /// <summary>
        /// �����L�[�������ꂽ���̊m�F
        /// </summary>
        /// <returns></returns>
        private int ItemNumberKeyDown()
        {
            if (Input.anyKeyDown)
            {
                for (int i = 49; i <= 55; i++)//1�L�[����7�L�[�܂ł͈̔͂�����
                {
                    if (Input.GetKeyDown((KeyCode)i))
                        return i;
                }
                return 0;
            }
            else return 0;
        }

        /// <summary>
        /// �A�C�e���X���b�g�̃��X�g��ύX�B
        /// </summary>
        /// <param name="index">�ύX���������X�g�̏���</param>
        /// <param name="value">�������\����</param>
        public void ChangeListValue(int index, ItemSlotStruct value)
        {
            _itemSlot[index] = value;
        }

        /// <summary>
        /// �A�C�e�����̂Ă�E�g���؂�Ƃ��ɌĂяo���BList�̕ύX�i�������j�Ɏg��
        /// </summary>
        /// <param name="index">�ύX���������X�g�̏���</param>
        private void ThrowItem(int index)
        {
            ItemSlotStruct temp = new ItemSlotStruct();
            _itemSlot[index] = temp;
        }

        /// <summary>
        /// �A�C�e���E���シ���ɃA�C�e�����g���Ȃ��悤�ɂ��邽�߂̃R���[�`��
        /// </summary>
        /// <returns></returns>
        public IEnumerator CanUseItem()
        {
            _isCanUseItem = false;
            yield return new WaitForSeconds(0.1f);
            _isCanUseItem = true;
        }
    }
}

