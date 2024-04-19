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


        public IObservable<int> OnNowIndexChange { get { return _nowIndex; } }//�O����_nowIndex�̒l���ύX���ꂽ�Ƃ��ɍs��������o�^�ł���悤�ɂ���
        public IObservable<String> OnItemPopActive { get { return _itemPopActive; } }
        public IObservable<CollectionReplaceEvent<ItemSlotStruct>> OnItemSlotReplace => _itemSlot.ObserveReplace();//�O����_itemSlot�̗v�f���ύX���ꂽ�Ƃ��ɍs��������o�^�ł���悤�ɂ���

        // Start is called before the first frame update
        void Start()
        {
            int layerMask = LayerMask.GetMask("Item");//Item�Ƃ������C���[�ɂ���GameObject�ɂ���ray��������Ȃ��悤�ɂ���

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
                            //�A�C�e���X���b�g����ł����p�\�ł���Ƃ��ɉE�N���b�N��������
                            if (_itemSlot[_nowIndex.Value].myItemData == null && _itemSlot[_nowIndex.Value].myItemSlotStatus == ItemSlotStatus.available && Input.GetMouseButtonDown(1))
                            {
                                ItemSlotStruct item = new ItemSlotStruct();
                                item.ChangeInfo(hit.collider.gameObject.GetComponent<ItemEffect>().GetItemData(), ItemSlotStatus.available);
                                ChangeListValue(_nowIndex.Value, item);//�A�C�e���X���b�g�ɃA�C�e�����i�[

                                Destroy(hit.collider.gameObject);//�X�e�[�W��ɂ���A�C�e����j��
                                StartCoroutine(CanUseItem());//�A�C�e���擾��0.1�b�̓A�C�e�����g���Ȃ��悤�ɂ���B
                            }
                        }
                        else
                        {
                            _itemPopActive.OnNext(null);//�A�C�e���|�b�v���A�N�e�B�u��
                        }
                    });

            //�E�N���b�N�����Ƃ��ɃA�C�e�����g�p            
            this.UpdateAsObservable()
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetMouseButtonDown(1) && _isCanUseItem)
                    .Subscribe(_ =>
                    {
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
        /// �A�C�e���X���b�g�̃��X�g��ύX�B���̂Ƃ���ʃX�N���v�g�Ŏg��Ȃ��̂�private�ɂ��Ă���
        /// </summary>
        /// <param name="index">�ύX���������X�g�̏���</param>
        /// <param name="value">�������\����</param>
        private void ChangeListValue(int index, ItemSlotStruct value)
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
            Debug.Log("�A�C�e���̂Ă�");
        }

        /// <summary>
        /// �A�C�e���E���シ���ɃA�C�e�����g���Ȃ��悤�ɂ��邽�߂̃R���[�`��
        /// </summary>
        /// <returns></returns>
        private IEnumerator CanUseItem()
        {
            _isCanUseItem = false;
            yield return new WaitForSeconds(0.1f);
            _isCanUseItem = true;
        }
    }
}

