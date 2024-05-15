using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �����̎�ނ�1��
    /// 1.���炷�X���b�g�ɃA�C�e���������Ă���Ƃ��̏�ɗ��Ƃ�
    /// 2.�g�̂̃}�q
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
            //�u���炷�X���b�g�ɃA�C�e���������Ă���Ƃ��̏�ɗ��Ƃ��v�����̎���
            for (int i = 0; i < 2; i++)
            {
                var itemSlot = _myPlayerItem.GetItemSlot(_randoms[i]);
                if (itemSlot.myItemData != null)
                {
                    //�A�C�e����^���ɗ��Ƃ�
                    _myPlayerItem.nowBringItem.transform.parent = null;
                    var rb = _myPlayerItem.nowBringItem.GetComponent<Rigidbody>();
                    rb.useGravity = true;

                    _myPlayerItem.nowBringItem = null;
                }

                //�����_����2�̃X���b�g�����������p�s�\�ɂ���
                _myPlayerItem.ChangeListValue(_randoms[i], _unavailableSlot);
            }

            //�u�g�̂̃}�q�v�@�\�̎���
            _myPlayerMove.isParalyzed = true;
        }

        public void Hide()
        {
            //���炷�X���b�g�ɃA�C�e���������Ă���Ƃ��̏�ɗ��Ƃ��v�����̉���
            for (int i = 0; i < 2; i++)
            {
                _myPlayerItem.ChangeListValue(_randoms[i], new ItemSlotStruct(null, ItemSlotStatus.available));
            }

            //�u�g�̂̃}�q�v����
            _myPlayerMove.isParalyzed = false;
        }
    }
}


