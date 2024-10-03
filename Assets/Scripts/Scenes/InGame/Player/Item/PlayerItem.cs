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
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Unity.Burst.CompilerServices;

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
        private bool _isCanChangeBringItem = true;//手に持つアイテムの変更を許可するか否か

        //Ray関連
        [SerializeField] Camera _mainCamera;//playerの目線を担うカメラ
        [SerializeField] private float _getItemRange;//アイテムを入手できる距離
        private bool _debugMode = false;

        //アイテムスロット（UI）の操作関連
        private float scrollValue;
        [SerializeField] private float scrollSense = 10;//マウスホイールの感度

        private bool _isCanUseItem = true;

        //UniRx関係
        private Subject<String> _popActive = new Subject<String>();
        private ReactiveCollection<ItemSlotStruct> _itemSlot = new ReactiveCollection<ItemSlotStruct>();//現在所持しているアイテムのリスト

        //アイテム表示切り替え用
        [SerializeField] private GameObject _compass;//Cameraに付属しているコンパス
        [SerializeField] private GameObject _thermometer;//Cameraに付属している温度計
        [SerializeField] private GameObject _geigerCounter;//Cameraに付属している放射線測定器

        //アイテムクラス関連
        [SerializeField] private Light _spotLight;
        [SerializeField] private ThermometerMove _thermometerMove;
        [SerializeField] private GeigerCounterMove _geigerCounterMove;
        
        //TrapFood関連
        [SerializeField] private GameObject _trapFood;
        private const float TILELENGTH = 5.85f;
        private GameObject _createdTrapFood;
        private bool _isCanCreateTrapFood;
        public bool IsCanCreateTrapFood { get => _isCanCreateTrapFood; }
        public GameObject CreatedTrapFood { get => _createdTrapFood; }

        //アイテムデバッグ用
        [SerializeField] private GameObject _itemForDebug;
       
        private List<HandLightState> _switchHandLight = new List<HandLightState>();//懐中電灯のon/off状態保存用
        private List<bool>_switchGeigerCounter = new List<bool>();//放射線測定器のon/off状態保存用  

        public List<ItemSlotStruct> ItemSlots { get { return _itemSlot.ToList(); } }//外部に_itemSlotの内容を公開する
        public int nowIndex { get => _nowIndex.Value; }
        public List<HandLightState> SwitchHandLights { get {  return _switchHandLight.ToList(); } }
        public List<bool> SwitchGeigerCounter { get { return _switchGeigerCounter.ToList(); } }

        public IObservable<int> OnNowIndexChange { get { return _nowIndex; } }//外部で_nowIndexの値が変更されたときに行う処理を登録できるようにする
        public IObservable<String> OnPopActive { get { return _popActive; } }
        public IObservable<CollectionReplaceEvent<ItemSlotStruct>> OnItemSlotReplace => _itemSlot.ObserveReplace();//外部に_itemSlotの要素が変更されたときに行う処理を登録できるようにする
        private Outlinable _lastOutlinable = null;
        private GameObject _lastGameobject = null;
        // Start is called before the first frame update
        void Start()
        {
            int layerMask = LayerMask.GetMask("Item") | LayerMask.GetMask("StageIntract") | LayerMask.GetMask("Wall");//Item, StageIntract,WallというレイヤーにあるGameObjectにしかrayが当たらないようにする
            _myPlayerStatus = GetComponent<PlayerStatus>();

            //今後はingame前のアイテムの所持状況を代入させる。α版は初期化
            ItemSlotStruct init = new ItemSlotStruct();
            for (int i = 0; i < 7; i++)
            {
                init.ChangeInfo();
                _itemSlot.Add(init);
            }

            //懐中電灯の状態をNotActiveでスロット分作っておく
            HandLightState LightSwitch = HandLightState.NotActive;
            for(int i = 0; i < 7; i++)
            {
                _switchHandLight.Add(LightSwitch);
                _switchGeigerCounter.Add(false);
            }

            //色々な変数の初期化
            scrollValue = 0;

            RaycastHit hit = new RaycastHit();
            //視線の先にアイテムがあるか確認。あれば右クリックで拾得できるようにする
            this.UpdateAsObservable()
                            .Where(_ => _myPlayerStatus.nowPlayerSurvive)
                            .Subscribe(_ =>
                            {
                                if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _getItemRange, layerMask))//設定した距離にあるアイテムを認知
                                {

                                    if (_debugMode)
                                    {
                                        Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward, Color.black);
                                    }
                                    //raycast先のオブジェクトが変化した際にOutlineを非表示にする

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
                                            //脱出アイテムだった時
                                            _lastOutlinable = hit.collider.gameObject.GetComponent<Outlinable>();
                                            IntractEvent(true, "脱出アイテム");//アウトライン表示
                                        }
                                        else if (hit.collider.gameObject.CompareTag("Item") && hit.collider.gameObject.TryGetComponent(out ItemEffect item))
                                        {
                                            //脱出アイテム以外のアイテムの時
                                            string name = item.GetItemData().itemName;
                                            _lastOutlinable = hit.collider.gameObject.GetComponent<Outlinable>();
                                            IntractEvent(true, name);//アウトライン表示
                                        }
                                        else if (hit.collider.gameObject.CompareTag("StageIntract"))
                                        {
                                            //StageIntract（ドアなど）のとき
                                            _lastOutlinable = hit.collider.gameObject.GetComponent<Outlinable>();
                                            IntractEvent(true, interactable.ReturnPopString());//アウトライン表示
                                        }
                                    }
                                }
                                else
                                {
                                    //Rayに何も当たらなかった時の処理
                                    IntractEvent(false, "");
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
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetKeyDown(KeyCode.H) && _isCanUseItem)
                    .Subscribe(_ =>
                    {
                        //アイテム捨てるときの処理
                        nowBringItem.GetComponent<ItemEffect>().OnThrow();

                        //アイテムを近くに投げ捨てる
                        var rb = nowBringItem.GetComponent<Rigidbody>();
                        nowBringItem.GetComponent<Collider>().enabled = true;
                        nowBringItem.transform.parent = null;
                        rb.useGravity = true;
                        rb.isKinematic = false;
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
                    if (nowBringItem != null)
                        Destroy(nowBringItem);

                    //手に選択したアイテムを出現させる
                    if (_itemSlot[_nowIndex.Value].myItemData != null)
                    {
                        nowBringItem = Instantiate(_itemSlot[_nowIndex.Value].myItemData.prefab, myRightHand.transform.position, _itemSlot[_nowIndex.Value].myItemData.prefab.transform.rotation);
                        nowBringItem.transform.parent = myRightHand.transform;
                        nowBringItem.GetComponent<ItemInstract>().InstantIntract(_myPlayerStatus);//アイテムに必要な情報を与える

                        //視覚上のバグを無くすために手に持っている間はColliderを消す
                        nowBringItem.GetComponent<Collider>().enabled = false;
                    }
                }).AddTo(this);

            //プレイヤーの入力による_nowIndexの変更
            //1.マウスホイールの入力
            //2.数字キーの入力
            this.UpdateAsObservable()
                    .Where(_ => Input.GetAxis("Mouse ScrollWheel") != 0 || ItemNumberKeyDown() != 0)
                    .Where(_ => _isCanChangeBringItem)
                    .Subscribe(_ =>
                    {
                        //マウスホールのみの入力時
                        if (ItemNumberKeyDown() == 0)
                        {
                            scrollValue -= Input.GetAxis("Mouse ScrollWheel") * scrollSense;
                            scrollValue = Mathf.Clamp(scrollValue, 0, 6);

                            if (_itemSlot[(int)scrollValue].myItemSlotStatus != ItemSlotStatus.unavailable)
                                _nowIndex.Value = (int)scrollValue;
                        }
                        //数字キーのみの入力時
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
                Debug.Log($"アイテム所持数：{y}");
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    {
                        if(_itemForDebug != null)
                        {
                            ItemSlotStruct item = new ItemSlotStruct();
                            item.ChangeInfo(_itemForDebug.GetComponent<ItemEffect>().GetItemData(), ItemSlotStatus.available);
                            ChangeListValue(0, item);
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
        /// アイテムを使い切るときに呼び出す。Listの変更（初期化）に使う
        /// </summary>
        /// <param name="index">変更したいリストの順番</param>
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

        public void CheckHaveDoll()
        {
            for (int i = 0; i < 7; i++)
            {
                if (_itemSlot[i].myItemData != null)
                {
                    if (_itemSlot[i].myItemData.itemID == 7)
                    {
                        //仮のアイテムを生成して、死亡時の効果を起動させる
                        GameObject Item = Instantiate(_itemSlot[i].myItemData.prefab);
                        Item.GetComponent<DollEffect>().UniqueEffect(_myPlayerStatus);

                        //アイテム破壊とアイテムスロットの初期化
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

        //懐中電灯を起動・停止するための関数
        public void ActiveHandLight(bool value)
        {
            _spotLight.enabled = value;
            _myPlayerStatus.ChangeLightRange(value);

        }


        //懐中電灯のON/OFFを切り替える関数
        public void ChangeSwitchHandLight(HandLightState state)
        {
            _switchHandLight[_nowIndex.Value] = state;
        }

        //コンパスを持つかどうか切り替える関数
        public void ActiveCompass(bool value)
        {
            _compass.SetActive(value);
        }


        //気温計を持つかどうか切り替える関数
        public void ActiveThermometer(bool value)
        {
            _thermometer.SetActive(value);
        }

        //気温計を使い測定を開始させる関数
        public void UseThermometer()
        {
            _thermometerMove.StartCoroutine("MeasureTemperature");
        }

        //放射線測定器を持つかどうか切り替える関数
        public void ActiveGeigerCounter(bool value)
        {
            _geigerCounter.SetActive(value);
        }

        //放射線測定器の電源のon/offを変更する関数
        public void ChangeSwitchGeigerCounter(bool value)
        {
            _switchGeigerCounter[_nowIndex.Value] = value;
        }

        //放射線測定器の動作状態を変更する関数
        public void UseGeigerCounter(bool value)
        {
            if (value)//測定を開始させる場合
            {
                _geigerCounterMove.StartCoroutine("MeasureGeigerCounter"); 
            }
            else//測定を止める場合
            {
                _geigerCounterMove.StopCoroutine("MeasureGeigerCounter");
                _geigerCounterMove.TurnOffGeigerCounter();
            }
         }   

        //地面判定を確認し、餌を生成する処理
        public IEnumerator CreateTrapFood()
        {
            RaycastHit hit;
            int floorlayerMask = LayerMask.GetMask("Floor");

            while (true)
            {
                yield return null;

                Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, TILELENGTH, floorlayerMask);
                Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * TILELENGTH, Color.black);

                if (hit.collider != null)
                {
                    //プレビューの作成・移動
                    if (_createdTrapFood == null)//プレビューがないときは作成
                    {
                        _createdTrapFood = Instantiate(_trapFood, hit.point, _trapFood.transform.rotation);
                    }
                    else if (_createdTrapFood.activeInHierarchy)// プレビューがありかつアクティブ状態の時
                    {
                        _createdTrapFood.transform.position = hit.point;
                    }
                    else// プレビューがあるがアクティブ状態でない時
                    {
                        _createdTrapFood.SetActive(true);
                        _createdTrapFood.GetComponent<TrapFoodCheckCollider>().ChangeTrigger(false);
                        _createdTrapFood.transform.position = hit.point;
                    }

                    if (_createdTrapFood.GetComponent<TrapFoodCheckCollider>().IsTriggered && _isCanCreateTrapFood)
                    {
                        _isCanCreateTrapFood = false;
                    }
                    else if (!_createdTrapFood.GetComponent<TrapFoodCheckCollider>().IsTriggered && !_isCanCreateTrapFood)
                    {
                        _isCanCreateTrapFood = true;
                    }
                }
                else
                {
                    _isCanCreateTrapFood = false;
                    if (_createdTrapFood != null)//プレビューが作成されていたら非アクティブにする
                    {
                        _createdTrapFood.SetActive(false);
                    }
                }

            }
        }

        //餌を設置する関数
        public void PutTrapFood()
        {
            _createdTrapFood.transform.GetChild(0).gameObject.SetActive(true);
            _createdTrapFood.layer = 10;// レイヤーをItemに変更し、拾えるようにする
            Instantiate(_createdTrapFood, _createdTrapFood.transform.position, _createdTrapFood.transform.rotation);
            Destroy(_createdTrapFood );       
            ConsumeItem(nowIndex);
        }

        //餌のアクティブ状態を切り替える関数
        public void ChangeActiveTrapFood(bool value)
        {
            if (_createdTrapFood != null)
            {
                _createdTrapFood.SetActive(value);
            }
        }

        public void DestroyTrapFood()
        {
            if ( _createdTrapFood != null ) 
            {
                Destroy(_createdTrapFood);

            }
        }
    }
}

