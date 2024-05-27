using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 発狂の種類の1つ
    /// 1.減らすスロットにアイテムが入っているとその場に落とす
    /// 2.身体のマヒ
    /// </summary>
    public class BodyParalyze : MonoBehaviour, IInsanity
    {
        private int[] _randoms = new int[2];
        private PlayerItem _myPlayerItem;
        private TempPlayerMove _myPlayerMove;

        private ItemSlotStruct _unavailableSlot;
        private bool _isFirst = true;//初めて呼び出されたか
        

        public void Setup()
        {
            _myPlayerItem = GetComponent<PlayerItem>();
            _myPlayerMove = GetComponent<TempPlayerMove>();
            _unavailableSlot = new ItemSlotStruct(null, ItemSlotStatus.unavailable);


            _randoms[0] = Random.Range(0, 7);
            while (true)
            {
                _randoms[1] = Random.Range(0, 7);
                if (_randoms[0] == _randoms[1])
                    continue;
                else
                    break;
            }

            Debug.Log("選ばれたスロットNo：" + _randoms[0] + " , " + _randoms[1]);
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }

            //「減らすスロットにアイテムが入っているとその場に落とす」処理の実装
            for (int i = 0; i < 2; i++)
            {
                var itemSlot = _myPlayerItem.ItemSlots[_randoms[i]];
                if (itemSlot.myItemData != null)
                {
                    //今手に持っているアイテムだった時
                    if (_randoms[i] == _myPlayerItem.nowIndex)
                    {
                        //アイテムを真下に落とす
                        _myPlayerItem.nowBringItem.transform.parent = null;
                        var rb = _myPlayerItem.nowBringItem.GetComponent<Rigidbody>();
                        rb.useGravity = true;

                        _myPlayerItem.nowBringItem = null;
                    }
                    else //手に持っていないアイテムだった時
                    {
                        Instantiate(itemSlot.myItemData.prefab, this.gameObject.transform.position + new Vector3(0,1,0), itemSlot.myItemData.prefab.transform.rotation);
                    }
                }

                //ランダムな2つのスロット初期化＆利用不可能にする
                _myPlayerItem.ChangeListValue(_randoms[i], _unavailableSlot);
            }

            //「身体のマヒ」機能の実装
            _myPlayerMove.Paralyze(true);
        }

        public void Hide()
        {
            //減らすスロットにアイテムが入っているとその場に落とす」処理の解除
            for (int i = 0; i < 2; i++)
            {
                _myPlayerItem.ChangeListValue(_randoms[i], new ItemSlotStruct(null, ItemSlotStatus.available));
            }

            //「身体のマヒ」解除
            _myPlayerMove.Paralyze(false);
            _myPlayerMove.MoveControl(true);
        }
    }
}


