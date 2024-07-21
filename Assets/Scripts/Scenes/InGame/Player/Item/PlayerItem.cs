using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;
using EPOOutline;
using Scenes.Ingame.InGameSystem;
using System.Diagnostics.Contracts;

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
        private bool _isCanChangeBringItem = true;//��Ɏ��A�C�e���̕ύX�������邩�ۂ�

        //Ray�֘A
        [SerializeField] Camera _mainCamera;//player�̖ڐ���S���J����
        [SerializeField] private float _getItemRange;//�A�C�e�������ł��鋗��
        private bool _debugMode = false;

        //�A�C�e���X���b�g�iUI�j�̑���֘A
        private float scrollValue;
        [SerializeField] private float scrollSense = 10;//�}�E�X�z�C�[���̊��x

        private bool _isCanUseItem = true;

        //UniRx�֌W
        private Subject<String> _popActive = new Subject<String>();
        private ReactiveCollection<ItemSlotStruct> _itemSlot = new ReactiveCollection<ItemSlotStruct>();//���ݏ������Ă���A�C�e���̃��X�g

        public List<ItemSlotStruct> ItemSlots { get { return _itemSlot.ToList(); } }//�O����_itemSlot�̓��e�����J����
        public int nowIndex { get => _nowIndex.Value; }


        public IObservable<int> OnNowIndexChange { get { return _nowIndex; } }//�O����_nowIndex�̒l���ύX���ꂽ�Ƃ��ɍs��������o�^�ł���悤�ɂ���
        public IObservable<String> OnPopActive { get { return _popActive; } }
        public IObservable<CollectionReplaceEvent<ItemSlotStruct>> OnItemSlotReplace => _itemSlot.ObserveReplace();//�O����_itemSlot�̗v�f���ύX���ꂽ�Ƃ��ɍs��������o�^�ł���悤�ɂ���
        private Outlinable _lastOutlinable = null;
        private GameObject _lastGameobject = null;

        [SerializeField] GameObject _itemForDebug;//�f�o�b�O�p�A�C�e��

        // Start is called before the first frame update
        void Start()
        {
            int layerMask = LayerMask.GetMask("Item") | LayerMask.GetMask("StageIntract") | LayerMask.GetMask("Wall");//Item, StageIntract,Wall�Ƃ������C���[�ɂ���GameObject�ɂ���ray��������Ȃ��悤�ɂ���
            _myPlayerStatus = GetComponent<PlayerStatus>();

            //�����ingame�O�̃A�C�e���̏����󋵂���������B���ł͏�����
            ItemSlotStruct init = new ItemSlotStruct();
            for (int i = 0; i < 7; i++)
            {
                init.ChangeInfo();
                _itemSlot.Add(init);
            }

            //�F�X�ȕϐ��̏�����
            scrollValue = 0;

            RaycastHit hit = new RaycastHit();
            //�����̐�ɃA�C�e�������邩�m�F�B����ΉE�N���b�N�ŏE���ł���悤�ɂ���
            this.UpdateAsObservable()
                .Where(_ => _myPlayerStatus.nowPlayerSurvive)
                .Subscribe(_ =>
                {
                    if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _getItemRange, layerMask))//�ݒ肵�������ɂ���A�C�e����F�m
                    {

                        if (_debugMode)
                        {
                            Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward, Color.black);
                        }
                        //raycast��̃I�u�W�F�N�g���ω������ۂ�Outline���\���ɂ���

                        if (_lastGameobject != null &&
                        _lastOutlinable != null &&
                        _lastGameobject != hit.collider.gameObject)
                        {
                            IntractEvent(false, "");
                        }
                        _lastGameobject = hit.collider.gameObject;

                        if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
                        {
                            interactable.Intract(_myPlayerStatus);

                            if (hit.collider.gameObject.CompareTag("Item") && hit.collider.gameObject.TryGetComponent(out EscapeItem escapeItem))
                            {
                                //�E�o�A�C�e����������
                                _lastOutlinable = hit.collider.gameObject.GetComponent<Outlinable>();
                                IntractEvent(true, "�E�o�A�C�e��");//�A�E�g���C���\��
                            }
                            else if (hit.collider.gameObject.CompareTag("Item") && hit.collider.gameObject.TryGetComponent(out ItemEffect item))
                            {
                                //�E�o�A�C�e���ȊO�̃A�C�e���̎�
                                string name = item.GetItemData().itemName;
                                _lastOutlinable = hit.collider.gameObject.GetComponent<Outlinable>();
                                IntractEvent(true, name);//�A�E�g���C���\��
                            }
                            else if (hit.collider.gameObject.CompareTag("StageIntract"))
                            {
                                //StageIntract�i�h�A�Ȃǁj�̂Ƃ�
                                _lastOutlinable = hit.collider.gameObject.GetComponent<Outlinable>();
                                IntractEvent(true, interactable.ReturnPopString());//�A�E�g���C���\��
                            }
                        }
                    }
                    else
                    {
                        //Ray�ɉ���������Ȃ��������̏���
                        IntractEvent(false, "");
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
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetKeyDown(KeyCode.H) && _isCanUseItem)
                    .Subscribe(_ =>
                    {
                        //�A�C�e���̂Ă�Ƃ��̏���
                        nowBringItem.GetComponent<ItemEffect>().OnThrow();

                        //�A�C�e�����߂��ɓ����̂Ă�
                        var rb = nowBringItem.GetComponent<Rigidbody>();
                        nowBringItem.GetComponent<Collider>().enabled = true;
                        nowBringItem.transform.parent = null;
                        rb.useGravity = true;
                        rb.isKinematic = false;
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
                    if (nowBringItem != null)
                        Destroy(nowBringItem);

                    //��ɑI�������A�C�e�����o��������
                    if (_itemSlot[_nowIndex.Value].myItemData != null)
                    {
                        nowBringItem = Instantiate(_itemSlot[_nowIndex.Value].myItemData.prefab, myRightHand.transform.position, _itemSlot[_nowIndex.Value].myItemData.prefab.transform.rotation);
                        nowBringItem.transform.parent = myRightHand.transform;
                        nowBringItem.GetComponent<ItemInstract>().InstantIntract(_myPlayerStatus);//�A�C�e���ɕK�v�ȏ���^����

                        //���o��̃o�O�𖳂������߂Ɏ�Ɏ����Ă���Ԃ�Collider������
                        nowBringItem.GetComponent<Collider>().enabled = false;
                    }
                }).AddTo(this);

            //�v���C���[�̓��͂ɂ��_nowIndex�̕ύX
            //1.�}�E�X�z�C�[���̓���
            //2.�����L�[�̓���
            this.UpdateAsObservable()
                    .Where(_ => Input.GetAxis("Mouse ScrollWheel") != 0 || ItemNumberKeyDown() != 0)
                    .Where(_ => _isCanChangeBringItem)
                    .Subscribe(_ =>
                    {
                        //�}�E�X�z�[���݂̂̓��͎�
                        if (ItemNumberKeyDown() == 0)
                        {
                            scrollValue -= Input.GetAxis("Mouse ScrollWheel") * scrollSense;
                            scrollValue = Mathf.Clamp(scrollValue, 0, 6);

                            if (_itemSlot[(int)scrollValue].myItemSlotStatus != ItemSlotStatus.unavailable)
                                _nowIndex.Value = (int)scrollValue;
                        }
                        //�����L�[�݂̂̓��͎�
                        if (Input.GetAxis("Mouse ScrollWheel") == 0)
                        {
                            int temp = ItemNumberKeyDown() - 49;

                            if (_itemSlot[temp].myItemSlotStatus != ItemSlotStatus.unavailable)
                            {
                                _nowIndex.Value = ItemNumberKeyDown() - 49;
                                scrollValue = _nowIndex.Value;
                            }
                        }
                    });

            
        }

        private void IntractEvent(bool outlineValue, string popString)
        {
            if(_lastOutlinable != null) 
                _lastOutlinable.enabled = outlineValue;

            _popActive.OnNext(popString);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                int y = 0;
                foreach (var x in _itemSlot)
                {
                    if (x.myItemData != null)
                    {
                        y += 1;
                    }
                }
                Debug.Log($"�A�C�e���������F{y}");
            }

            if(Input.GetKeyDown(KeyCode.B))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    {
                        

                        if(_itemForDebug != null)
                        {
                            ItemSlotStruct item = new ItemSlotStruct();
                            item.ChangeInfo(_itemForDebug.GetComponent<ItemEffect>().GetItemData(), ItemSlotStatus.available);
                            if (_itemSlot[0].myItemData != null)
                            {
                                ChangeListValue(1, item);
                            }
                            else
                            {
                                ChangeListValue(0, item);
                            }

                            
                            nowBringItem = Instantiate(_itemForDebug);


                            nowBringItem.gameObject.transform.position = myRightHand.transform.position;
                            nowBringItem.gameObject.transform.parent = myRightHand.transform;
                            var effect = nowBringItem.gameObject.GetComponent<ItemEffect>();
                            effect.ownerPlayerStatus = _myPlayerStatus;
                            effect.ownerPlayerItem = this;
                            effect.OnPickUp();
                            var rigid = nowBringItem.GetComponent<Rigidbody>();
                            rigid.useGravity = false;
                            rigid.isKinematic = true;
                        }


                    }
                }
            }


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
        /// �A�C�e�����g���؂�Ƃ��ɌĂяo���BList�̕ύX�i�������j�Ɏg��
        /// </summary>
        /// <param name="index">�ύX���������X�g�̏���</param>
        public void ConsumeItem(int index)
        {
            if (nowBringItem != null)
                Destroy(nowBringItem);

            ItemSlotStruct temp = new ItemSlotStruct();
            _itemSlot[index] = temp;
        }

        public void ChangeCanUseItem(bool value)
        {
            _isCanUseItem = value;
        }

        public void ChangeCanChangeBringItem(bool value)
        {
            _isCanChangeBringItem = value;
        }

        /// <summary>
        /// �g����l�`�������Ă��邩�m�F���邽�߂̊֐�
        /// </summary>
        public void CheckHaveDoll()
        {
            for (int i = 0; i < 7; i++)
            {                    
                if (_itemSlot[i].myItemData != null)
                {
                    if (_itemSlot[i].myItemData.itemID == 7)
                    {
                        //���̃A�C�e���𐶐����āA���S���̌��ʂ��N��������
                        GameObject Item = Instantiate(_itemSlot[i].myItemData.prefab);
                        Item.GetComponent<DollEffect>().UniqueEffect(_myPlayerStatus);

                        //�A�C�e���j��ƃA�C�e���X���b�g�̏�����
                        Destroy(Item);
                        if (_nowIndex.Value == i && nowBringItem != null)
                        {
                            Destroy(nowBringItem);
                        }
                        ItemSlotStruct temp = new ItemSlotStruct();
                        _itemSlot[i] = temp;

                        break;
                    }

                }

            }

        }
    }
}

