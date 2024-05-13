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
    /// アイテムに関する処理をまとめたクラス
    /// 1.アイテムスロットにあるアイテムを使用する
    /// 2.所持アイテムの管理
    /// 3.アイテムスロットの位置の管理
    /// </summary>
    public class PlayerItem : MonoBehaviour
    {
        private PlayerStatus _myPlayerStatus;

        //アイテム関係
        private ReactiveProperty<int> _nowIndex = new ReactiveProperty<int>();//選択中のアイテムスロット番号
        public GameObject myRightHand;//手のこと
        public GameObject nowBringItem;//現在手に持っているアイテム
        public bool isCanChangeBringItem = true;//手に持つアイテムの変更を許可するか否か

        //Ray関連
        [SerializeField] Camera _mainCamera;//playerの目線を担うカメラ
        [SerializeField] private float _getItemRange;//アイテムを入手できる距離

        //アイテムスロット（UI）の操作関連
        private float scrollValue;
        [SerializeField] private float scrollSense = 10;//マウスホイールの感度

        private bool _isCanUseItem = true;

        //UniRx関係
        private Subject<String> _itemPopActive = new Subject<String>();
        private ReactiveCollection<ItemSlotStruct> _itemSlot = new ReactiveCollection<ItemSlotStruct>();//現在所持しているアイテムのリスト

        public List<ItemSlotStruct> ItemSlots { get { return _itemSlot.ToList(); } }//外部に_itemSlotの内容を公開する
        public int nowIndex { get => _nowIndex.Value; }


        public IObservable<int> OnNowIndexChange { get { return _nowIndex; } }//外部で_nowIndexの値が変更されたときに行う処理を登録できるようにする
        public IObservable<String> OnItemPopActive { get { return _itemPopActive; } }
        public IObservable<CollectionReplaceEvent<ItemSlotStruct>> OnItemSlotReplace => _itemSlot.ObserveReplace();//外部に_itemSlotの要素が変更されたときに行う処理を登録できるようにする


        // Start is called before the first frame update
        void Start()
        {
            int layerMask = LayerMask.GetMask("Item");//ItemというレイヤーにあるGameObjectにしかrayが当たらないようにする
            _myPlayerStatus = GetComponent<PlayerStatus>();

            //今後はingame前のアイテムの所持状況を代入させる。α版は初期化
            ItemSlotStruct init = new ItemSlotStruct();            
            for (int i = 0; i < 7; i++)
            {
                init.ChangeInfo();
                _itemSlot.Add(init);
            }

            //視線の先にアイテムがあるか確認。あれば右クリックで拾得できるようにする
            this.UpdateAsObservable()
                    .Subscribe(_ =>
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _getItemRange, layerMask))//設定した距離にあるアイテムを認知
                        {
                            if(hit.collider.gameObject.TryGetComponent(out ItemEffect item))
                            {
                                string name = item.GetItemData().itemName;
                                _itemPopActive.OnNext(name);//アイテムポップが出現
                            }
                            //TryGetComponentを行う。
                            if (hit.collider.gameObject.TryGetComponent(out IInteractable intract))
                            {
                                intract.Intract(_myPlayerStatus);
                            }
                        }
                        else
                        {
                            _itemPopActive.OnNext(null);//アイテムポップを非アクティブ化
                        }
                    });

            //左クリックしたときにアイテムを使用
            this.UpdateAsObservable()
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetMouseButtonDown(0) && _isCanUseItem)
                    .Subscribe(_ =>
                    {
                        Debug.Log("アイテム使う");

                        //アイテムを使用
                        nowBringItem.GetComponent<ItemEffect>().Effect();
                    });

            //Hキーを入力したときにアイテムを破棄            
            this.UpdateAsObservable()
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetKeyDown(KeyCode.H))
                    .Subscribe(_ =>
                    {
                        var rb = nowBringItem.GetComponent<Rigidbody>();
                        //アイテムを近くに投げ捨てる
                        nowBringItem.transform.parent = null;
                        rb.useGravity = true;
                        rb.AddForce(_mainCamera.transform.forward * 300);
                        
                        //アイテムスロットのListを更新
                        nowBringItem = null;
                        ItemSlotStruct temp = new ItemSlotStruct();
                        _itemSlot[_nowIndex.Value] = temp;
                    });

            //アイテムスロットの選択状態が変わったときに、手元に適切なアイテムを出現させる
            _nowIndex
                .Subscribe(_ => 
                {
                    //他にアイテムを手に持っていたら、それを破壊
                    if(nowBringItem != null)
                        Destroy(nowBringItem);

                    //手に選択したアイテムを出現させる
                    if (_itemSlot[_nowIndex.Value].myItemData != null)
                    {
                        nowBringItem = Instantiate(_itemSlot[_nowIndex.Value].myItemData.prefab, myRightHand.transform.position, _itemSlot[_nowIndex.Value].myItemData.prefab.transform.rotation);
                        nowBringItem.transform.parent = myRightHand.transform;
                        nowBringItem.GetComponent<ItemInstract>().InstantIntract(_myPlayerStatus);//アイテムに必要な情報を与える
                    }
                }).AddTo(this);

            //プレイヤーの入力による_nowIndexの変更
            //1.マウスホイールの入力
            //2.数字キーの入力
            this.UpdateAsObservable()
                    .Where(_ => Input.GetAxis("Mouse ScrollWheel") != 0 || ItemNumberKeyDown() != 0)
                    .Where(_ => isCanChangeBringItem)
                    .Subscribe(_ =>
                    {
                        //マウスホールのみの入力時
                        if (ItemNumberKeyDown() == 0)
                        {
                            scrollValue += Input.GetAxis("Mouse ScrollWheel") * scrollSense;
                            scrollValue = Mathf.Clamp(scrollValue,0,6);
                            _nowIndex.Value = (int)scrollValue;
                        }
                        //数字キーのみの入力時
                        if (Input.GetAxis("Mouse ScrollWheel") == 0)
                        {
                            _nowIndex.Value = ItemNumberKeyDown() - 49;
                            scrollValue = _nowIndex.Value;
                        }                           
                    });
        }

        /// <summary>
        /// 数字キーが押されたかの確認
        /// </summary>
        /// <returns></returns>
        private int ItemNumberKeyDown()
        {
            if (Input.anyKeyDown)
            {
                for (int i = 49; i <= 55; i++)//1キーから7キーまでの範囲を検索
                {
                    if (Input.GetKeyDown((KeyCode)i))
                        return i;
                }
                return 0;
            }
            else return 0;
        }

        /// <summary>
        /// アイテムスロットのリストを変更。
        /// </summary>
        /// <param name="index">変更したいリストの順番</param>
        /// <param name="value">代入する構造体</param>
        public void ChangeListValue(int index, ItemSlotStruct value)
        {
            _itemSlot[index] = value;
        }

        /// <summary>
        /// アイテムを捨てる・使い切るときに呼び出す。Listの変更（初期化）に使う
        /// </summary>
        /// <param name="index">変更したいリストの順番</param>
        public void ThrowItem(int index)
        {
            if(nowBringItem != null)
                Destroy(nowBringItem);

            ItemSlotStruct temp = new ItemSlotStruct();
            _itemSlot[index] = temp;
        }

        /// <summary>
        /// アイテム拾得後すぐにアイテムを使えないようにするためのコルーチン
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

