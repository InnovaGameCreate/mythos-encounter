using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;
using Unity.VisualScripting;

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
        public GameObject myRightHand;//��̂���
        public GameObject nowBringItem;//���ݎ�Ɏ����Ă���A�C�e��
        public bool isCanChangeBringItem = true;//��Ɏ��A�C�e���̕ύX�������邩�ۂ�

        //Ray�֘A
        [SerializeField] Camera _mainCamera;//player�̖ڐ���S���J����
        [SerializeField] private float _getItemRange;//�A�C�e�������ł��鋗��

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
                init.ChangeInfo();
                _itemSlot.Add(init);
            }

            //�����̐�ɃA�C�e�������邩�m�F�B����ΉE�N���b�N�ŏE���ł���悤�ɂ���
            this.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _getItemRange, layerMask))//�ݒ肵�������ɂ���A�C�e����F�m
                        {
                            if(hit.collider.gameObject.TryGetComponent(out ItemEffect item))
                            {
                                string name = item.GetItemData().itemName;
                                _itemPopActive.OnNext(name);//�A�C�e���|�b�v���o��
                            }
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
                        nowBringItem.GetComponent<ItemEffect>().Effect();
                    });

            //H�L�[����͂����Ƃ��ɃA�C�e����j��            
            this.UpdateAsObservable()
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetKeyDown(KeyCode.H))
                    .Subscribe(_ =>
                    {
                        var rb = nowBringItem.GetComponent<Rigidbody>();
                        //�A�C�e�����߂��ɓ����̂Ă�
                        nowBringItem.transform.parent = null;
                        rb.useGravity = true;
                        rb.AddForce(_mainCamera.transform.forward * 300);
                        
                        //�A�C�e���X���b�g��List���X�V
                        nowBringItem = null;
                        ItemSlotStruct temp = new ItemSlotStruct();
                        _itemSlot[_nowIndex.Value] = temp;
                    });

            //�A�C�e���X���b�g�̑I����Ԃ��ς�����Ƃ��ɁA�茳�ɓK�؂ȃA�C�e�����o��������
            _nowIndex
                .Subscribe(_ => 
                {
                    //���ɃA�C�e������Ɏ����Ă�����A�����j��
                    if(nowBringItem != null)
                        Destroy(nowBringItem);

                    //��ɑI�������A�C�e�����o��������
                    if (_itemSlot[_nowIndex.Value].myItemData != null)
                    {
                        nowBringItem = Instantiate(_itemSlot[_nowIndex.Value].myItemData.prefab, myRightHand.transform.position, _itemSlot[_nowIndex.Value].myItemData.prefab.transform.rotation);
                        nowBringItem.transform.parent = myRightHand.transform;
                        nowBringItem.GetComponent<ItemInstract>().InstantIntract(_myPlayerStatus);//�A�C�e���ɕK�v�ȏ���^����
                    }
                }).AddTo(this);

            //�v���C���[�̓��͂ɂ��_nowIndex�̕ύX
            //1.�}�E�X�z�C�[���̓���
            //2.�����L�[�̓���
            this.UpdateAsObservable()
                    .Where(_ => Input.GetAxis("Mouse ScrollWheel") != 0 || ItemNumberKeyDown() != 0)
                    .Where(_ => isCanChangeBringItem)
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
        public void ThrowItem(int index)
        {
            if(nowBringItem != null)
                Destroy(nowBringItem);

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

