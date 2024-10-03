using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �����̎�ނ�1��
    /// 1.�v���C���[��5�b�Ԑ����グ�Ĕ�������B���̂Ƃ����������A�j���[�V�����Ɉڍs
    /// 2.��؂̍s�����s��
    /// </summary>
    public class Scream : MonoBehaviour, IInsanity
    {
        private PlayerSoundManager _myPlayerSoundManager;
        private PlayerMove _myPlayerMove;
        private PlayerItem _myPlayerItem;

        private bool _isSafetyBool = false;//��������ł���Ƃ��Ɏ�Ⴂ��SAN�l���񕜂��A���̃X�N���v�g���j�󂳂ꂽ�Ƃ��ɋl�܂Ȃ��ׂ�Bool
        private bool _isFirst = true;//���߂ČĂяo���ꂽ��

        public void Setup()
        {
            _myPlayerSoundManager = GetComponent<PlayerSoundManager>();
            _myPlayerMove = GetComponent<PlayerMove>();
            _myPlayerItem = GetComponent<PlayerItem>();
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }

            //�������͍s���s�\�ɂȂ�
            _myPlayerMove.MoveControl(false);
            _myPlayerItem.ChangeCanUseItem(false);
            _myPlayerItem.ChangeCanChangeBringItem(false);
            _isSafetyBool = true;

            //���ѐ����グ��
            _myPlayerSoundManager.SetScreamClip("Male");

            //���������A�j���[�V�����ɑJ��
            //�������

            StartCoroutine(FinishScream());
        }

        public void Hide()
        {
            if (_isSafetyBool)
            {
                _myPlayerMove.MoveControl(true);
                _myPlayerItem.ChangeCanUseItem(true);
                _myPlayerItem.ChangeCanChangeBringItem(true);
            }

        }

        /// <summary>
        /// ���Ԃ��Ƃ��I���΍s���s�\������
        /// </summary>
        /// <returns></returns>
        private IEnumerator FinishScream()
        {
            yield return new WaitForSeconds(5.0f);
            _myPlayerMove.MoveControl(true);
            _myPlayerItem.ChangeCanUseItem(true);
            _myPlayerItem.ChangeCanChangeBringItem(true);

            _isSafetyBool = false;
        }
    }
}
