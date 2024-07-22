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

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 繧｢繧､繝�Β縺ｫ髢｢縺吶ｋ蜃ｦ逅�ｒ縺ｾ縺ｨ繧√◆繧ｯ繝ｩ繧ｹ
    /// 1.繧｢繧､繝�Β繧ｹ繝ｭ繝�ヨ縺ｫ縺ゅｋ繧｢繧､繝�Β繧剃ｽｿ逕ｨ縺吶ｋ
    /// 2.謇謖√い繧､繝�Β縺ｮ邂｡逅
    /// 3.繧｢繧､繝�Β繧ｹ繝ｭ繝�ヨ縺ｮ菴咲ｽｮ縺ｮ邂｡逅
    /// </summary>
    public class PlayerItem : MonoBehaviour
    {
        private PlayerStatus _myPlayerStatus;

        //繧｢繧､繝�Β髢｢菫
        private ReactiveProperty<int> _nowIndex = new ReactiveProperty<int>();//驕ｸ謚樔ｸｭ縺ｮ繧｢繧､繝�Β繧ｹ繝ｭ繝�ヨ逡ｪ蜿ｷ
        public GameObject myRightHand;//謇九�縺薙→
        public GameObject nowBringItem;//迴ｾ蝨ｨ謇九↓謖√▲縺ｦ縺�ｋ繧｢繧､繝�Β
        private bool _isCanChangeBringItem = true;//謇九↓謖√▽繧｢繧､繝�Β縺ｮ螟画峩繧定ｨｱ蜿ｯ縺吶ｋ縺句凄縺

        //Ray髢｢騾｣
        [SerializeField] Camera _mainCamera;//player縺ｮ逶ｮ邱壹ｒ諡�≧繧ｫ繝｡繝ｩ
        [SerializeField] private float _getItemRange;//繧｢繧､繝�Β繧貞�謇九〒縺阪ｋ霍晞屬
        private bool _debugMode = false;

        //繧｢繧､繝�Β繧ｹ繝ｭ繝�ヨ��I�峨�謫堺ｽ憺未騾｣
        private float scrollValue;
        [SerializeField] private float scrollSense = 10;//繝槭え繧ｹ繝帙う繝ｼ繝ｫ縺ｮ諢溷ｺｦ

        private bool _isCanUseItem = true;

        //UniRx髢｢菫
        private Subject<String> _popActive = new Subject<String>();
        private ReactiveCollection<ItemSlotStruct> _itemSlot = new ReactiveCollection<ItemSlotStruct>();//迴ｾ蝨ｨ謇謖√＠縺ｦ縺�ｋ繧｢繧､繝�Β縺ｮ繝ｪ繧ｹ繝

        [SerializeField] private GameObject _spotLight;//Camera縺ｫ莉伜ｱ槭＠縺ｦ縺�ｋ繧ｹ繝昴ャ繝医Λ繧､繝

        //繧｢繧､繝�Β繝�ヰ繝�げ逕ｨ
        [SerializeField] private GameObject _itemForDebug;

        //諛蝉ｸｭ髮ｻ轣ｯ縺ｮon/off迥ｶ諷倶ｿ晏ｭ倡畑
        private List<HandLightState> _switchHandLight = new List<HandLightState>();

        public List<ItemSlotStruct> ItemSlots { get { return _itemSlot.ToList(); } }//螟夜Κ縺ｫ_itemSlot縺ｮ蜀�ｮｹ繧貞�髢九☆繧
        public int nowIndex { get => _nowIndex.Value; }
        public List<HandLightState> SwitchHandLights { get {  return _switchHandLight.ToList(); } }


        public IObservable<int> OnNowIndexChange { get { return _nowIndex; } }//螟夜Κ縺ｧ_nowIndex縺ｮ蛟､縺悟､画峩縺輔ｌ縺溘→縺阪↓陦後≧蜃ｦ逅�ｒ逋ｻ骭ｲ縺ｧ縺阪ｋ繧医≧縺ｫ縺吶ｋ
        public IObservable<String> OnPopActive { get { return _popActive; } }
        public IObservable<CollectionReplaceEvent<ItemSlotStruct>> OnItemSlotReplace => _itemSlot.ObserveReplace();//螟夜Κ縺ｫ_itemSlot縺ｮ隕∫ｴ縺悟､画峩縺輔ｌ縺溘→縺阪↓陦後≧蜃ｦ逅�ｒ逋ｻ骭ｲ縺ｧ縺阪ｋ繧医≧縺ｫ縺吶ｋ
        private Outlinable _lastOutlinable = null;
        private GameObject _lastGameobject = null;

        [SerializeField] GameObject _itemForDebug;//繝�ヰ繝�げ逕ｨ繧｢繧､繝�Β

        // Start is called before the first frame update
        void Start()
        {
            int layerMask = LayerMask.GetMask("Item") | LayerMask.GetMask("StageIntract") | LayerMask.GetMask("Wall");//Item, StageIntract,Wall縺ｨ縺�≧繝ｬ繧､繝､繝ｼ縺ｫ縺ゅｋGameObject縺ｫ縺励°ray縺悟ｽ薙◆繧峨↑縺�ｈ縺�↓縺吶ｋ
            _myPlayerStatus = GetComponent<PlayerStatus>();

            //莉雁ｾ後�ingame蜑阪�繧｢繧､繝�Β縺ｮ謇謖∫憾豕√ｒ莉｣蜈･縺輔○繧九ばｱ迚医�蛻晄悄蛹
            ItemSlotStruct init = new ItemSlotStruct();
            for (int i = 0; i < 7; i++)
            {
                init.ChangeInfo();
                _itemSlot.Add(init);
            }

            //諛蝉ｸｭ髮ｻ轣ｯ縺ｮ迥ｶ諷九ｒNotActive縺ｧ繧ｹ繝ｭ繝�ヨ蛻�ｽ懊▲縺ｦ縺翫￥
            HandLightState LightSwitch = HandLightState.NotActive;
            for(int i = 0; i < 7; i++)
            {
                _switchHandLight.Add(LightSwitch);
            }

            //濶ｲ縲�↑螟画焚縺ｮ蛻晄悄蛹
            scrollValue = 0;

            RaycastHit hit = new RaycastHit();
            //隕也ｷ壹�蜈医↓繧｢繧､繝�Β縺後≠繧九°遒ｺ隱阪ゅ≠繧後�蜿ｳ繧ｯ繝ｪ繝�け縺ｧ諡ｾ蠕励〒縺阪ｋ繧医≧縺ｫ縺吶ｋ
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

            //蟾ｦ繧ｯ繝ｪ繝�け縺励◆縺ｨ縺阪↓繧｢繧､繝�Β繧剃ｽｿ逕ｨ
            this.UpdateAsObservable()
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetMouseButtonDown(0) && _isCanUseItem)
                    .Subscribe(_ =>
                    {
                        Debug.Log("繧｢繧､繝�Β菴ｿ縺");

                        //繧｢繧､繝�Β繧剃ｽｿ逕ｨ
                        nowBringItem.GetComponent<ItemEffect>().Effect();
                    });

            //H繧ｭ繝ｼ繧貞�蜉帙＠縺溘→縺阪↓繧｢繧､繝�Β繧堤ｴ譽            
            this.UpdateAsObservable()
                    .Where(_ => _itemSlot[_nowIndex.Value].myItemData != null && Input.GetKeyDown(KeyCode.H) && _isCanUseItem)
                    .Subscribe(_ =>
                    {
                        //繧｢繧､繝�Β謐ｨ縺ｦ繧九→縺阪�蜃ｦ逅
                        nowBringItem.GetComponent<ItemEffect>().OnThrow();

                        //繧｢繧､繝�Β繧定ｿ代￥縺ｫ謚輔￡謐ｨ縺ｦ繧
                        var rb = nowBringItem.GetComponent<Rigidbody>();
                        nowBringItem.GetComponent<Collider>().enabled = true;
                        nowBringItem.transform.parent = null;
                        rb.useGravity = true;
                        rb.isKinematic = false;
                        rb.AddForce(_mainCamera.transform.forward * 300);

                        //繧｢繧､繝�Β繧ｹ繝ｭ繝�ヨ縺ｮList繧呈峩譁ｰ
                        nowBringItem = null;
                        ItemSlotStruct temp = new ItemSlotStruct();
                        _itemSlot[_nowIndex.Value] = temp;
                    });

            //繧｢繧､繝�Β繧ｹ繝ｭ繝�ヨ縺ｮ驕ｸ謚樒憾諷九′螟峨ｏ縺｣縺溘→縺阪↓縲∵焔蜈�↓驕ｩ蛻�↑繧｢繧､繝�Β繧貞�迴ｾ縺輔○繧
            _nowIndex
                .Subscribe(_ =>
                {
                    //莉悶↓繧｢繧､繝�Β繧呈焔縺ｫ謖√▲縺ｦ縺�◆繧峨√◎繧後ｒ遐ｴ螢
                    if (nowBringItem != null)
                        Destroy(nowBringItem);

                    //謇九↓驕ｸ謚槭＠縺溘い繧､繝�Β繧貞�迴ｾ縺輔○繧
                    if (_itemSlot[_nowIndex.Value].myItemData != null)
                    {
                        nowBringItem = Instantiate(_itemSlot[_nowIndex.Value].myItemData.prefab, myRightHand.transform.position, _itemSlot[_nowIndex.Value].myItemData.prefab.transform.rotation);
                        nowBringItem.transform.parent = myRightHand.transform;
                        nowBringItem.GetComponent<ItemInstract>().InstantIntract(_myPlayerStatus);//繧｢繧､繝�Β縺ｫ蠢�ｦ√↑諠�ｱ繧剃ｸ弱∴繧

                        //隕冶ｦ壻ｸ翫�繝舌げ繧堤┌縺上☆縺溘ａ縺ｫ謇九↓謖√▲縺ｦ縺�ｋ髢薙�Collider繧呈ｶ医☆
                        nowBringItem.GetComponent<Collider>().enabled = false;
                    }
                }).AddTo(this);

            //繝励Ξ繧､繝､繝ｼ縺ｮ蜈･蜉帙↓繧医ｋ_nowIndex縺ｮ螟画峩
            //1.繝槭え繧ｹ繝帙う繝ｼ繝ｫ縺ｮ蜈･蜉
            //2.謨ｰ蟄励く繝ｼ縺ｮ蜈･蜉
            this.UpdateAsObservable()
                    .Where(_ => Input.GetAxis("Mouse ScrollWheel") != 0 || ItemNumberKeyDown() != 0)
                    .Where(_ => _isCanChangeBringItem)
                    .Subscribe(_ =>
                    {
                        //繝槭え繧ｹ繝帙�繝ｫ縺ｮ縺ｿ縺ｮ蜈･蜉帶凾
                        if (ItemNumberKeyDown() == 0)
                        {
                            scrollValue -= Input.GetAxis("Mouse ScrollWheel") * scrollSense;
                            scrollValue = Mathf.Clamp(scrollValue, 0, 6);

                            if (_itemSlot[(int)scrollValue].myItemSlotStatus != ItemSlotStatus.unavailable)
                                _nowIndex.Value = (int)scrollValue;
                        }
                        //謨ｰ蟄励く繝ｼ縺ｮ縺ｿ縺ｮ蜈･蜉帶凾
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
                Debug.Log($"繧｢繧､繝�Β謇謖∵焚�嘴y}");
            }


            if(Input.GetKeyDown(KeyCode.B))
            {
                if (Input.GetKey(KeyCode.LeftShift))
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

        /// <summary>
        /// 謨ｰ蟄励く繝ｼ縺梧款縺輔ｌ縺溘°縺ｮ遒ｺ隱
        /// </summary>
        /// <returns></returns>
        private int ItemNumberKeyDown()
        {
            if (Input.anyKeyDown)
            {
                for (int i = 49; i <= 55; i++)//1繧ｭ繝ｼ縺九ｉ7繧ｭ繝ｼ縺ｾ縺ｧ縺ｮ遽�峇繧呈､懃ｴ｢
                {
                    if (Input.GetKeyDown((KeyCode)i))
                        return i;
                }
                return 0;
            }
            else return 0;
        }

        /// <summary>
        /// 繧｢繧､繝�Β繧ｹ繝ｭ繝�ヨ縺ｮ繝ｪ繧ｹ繝医ｒ螟画峩縲
        /// </summary>
        /// <param name="index">螟画峩縺励◆縺�Μ繧ｹ繝医�鬆�分</param>
        /// <param name="value">莉｣蜈･縺吶ｋ讒矩菴</param>
        public void ChangeListValue(int index, ItemSlotStruct value)
        {
            _itemSlot[index] = value;
        }

        /// <summary>
        /// 繧｢繧､繝�Β繧剃ｽｿ縺��繧九→縺阪↓蜻ｼ縺ｳ蜃ｺ縺吶�ist縺ｮ螟画峩�亥�譛溷喧�峨↓菴ｿ縺
        /// </summary>
        /// <param name="index">螟画峩縺励◆縺�Μ繧ｹ繝医�鬆�分</param>
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

        //諛蝉ｸｭ髮ｻ轣ｯ繧定ｵｷ蜍輔�蛛懈ｭ｢縺吶ｋ縺溘ａ縺ｮ髢｢謨ｰ
        public void ActiveHandLight(bool value)
        {
            _spotLight.GetComponent<Light>().enabled = value;
            _myPlayerStatus.ChangeLightRange(value);
            
        }

        //諛蝉ｸｭ髮ｻ轣ｯ縺ｮON/OFF繧貞�繧頑崛縺医ｋ髢｢謨ｰ
        public void ChangeSwitchHandLight(HandLightState state)
        {
            _switchHandLight[_nowIndex.Value] = state;
        }

        /// <summary>
        /// 霄ｫ莉｣繧上ｊ莠ｺ蠖｢繧呈戟縺｣縺ｦ縺�ｋ縺狗｢ｺ隱阪☆繧九◆繧√�髢｢謨ｰ
        /// </summary>
        public void CheckHaveDoll()
        {
            for (int i = 0; i < 7; i++)
            {                    
                if (_itemSlot[i].myItemData != null)
                {
                    if (_itemSlot[i].myItemData.itemID == 7)
                    {
                        //莉ｮ縺ｮ繧｢繧､繝�Β繧堤函謌舌＠縺ｦ縲∵ｭｻ莠｡譎ゅ�蜉ｹ譫懊ｒ襍ｷ蜍輔＆縺帙ｋ
                        GameObject Item = Instantiate(_itemSlot[i].myItemData.prefab);
                        Item.GetComponent<DollEffect>().UniqueEffect(_myPlayerStatus);

                        //繧｢繧､繝�Β遐ｴ螢翫→繧｢繧､繝�Β繧ｹ繝ｭ繝�ヨ縺ｮ蛻晄悄蛹
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

