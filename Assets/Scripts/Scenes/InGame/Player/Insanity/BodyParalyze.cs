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

        private void Start()
        {
            Setup();
            _myPlayerItem = GetComponent<PlayerItem>();
            _myPlayerMove = GetComponent<TempPlayerMove>();

            _unavailableSlot = new ItemSlotStruct(null,ItemSlotStatus.unavailable);
        }

        public void Setup()
        {
            _randoms[0] = Random.Range(0, 7);
            while (true)
            {
                _randoms[1] = Random.Range(0, 7);
                if (_randoms[0] == _randoms[1])
                    continue;
                else
                    break;
            }
        }

        public void Active()
        {
            //「減らすスロットにアイテムが入っているとその場に落とす」処理の実装
            for (int i = 0; i < 2; i++)
            {
                var itemSlot = _myPlayerItem.GetItemSlot(_randoms[i]);
                if (itemSlot.myItemData != null)
                {
                    //アイテムを真下に落とす
                    _myPlayerItem.nowBringItem.transform.parent = null;
                    var rb = _myPlayerItem.nowBringItem.GetComponent<Rigidbody>();
                    rb.useGravity = true;

                    _myPlayerItem.nowBringItem = null;
                }

                //ランダムな2つのスロット初期化＆利用不可能にする
                _myPlayerItem.ChangeListValue(_randoms[i], _unavailableSlot);
            }

            //「身体のマヒ」機能の実装
            _myPlayerMove.isParalyzed = true;
        }

        public void Hide()
        {
            //減らすスロットにアイテムが入っているとその場に落とす」処理の解除
            for (int i = 0; i < 2; i++)
            {
                _myPlayerItem.ChangeListValue(_randoms[i], new ItemSlotStruct(null, ItemSlotStatus.available));
            }

            //「身体のマヒ」解除
            _myPlayerMove.isParalyzed = false;
        }
    }
}


