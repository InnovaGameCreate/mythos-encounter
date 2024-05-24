using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �����̎�ނ�1��
    /// 1.�X�^�~�i�̏���x�Əo���Ŏ󂯂�_���[�W�̌������x��2�{�ɂȂ�.
    /// 2.���ǂ���ƃv���C���[��1�b��1�x�̃y�[�X�Ŕ�������f���悤�ɂȂ�.�S�����̑���
    /// </summary>
    public class IncreasePulsation : MonoBehaviour,IInsanity
    {
        private PlayerStatus _myPlayerStatus;
        private TempPlayerMove _myPlayerMove;

        private bool _isFirst = true;//���߂ČĂяo���ꂽ��


        public void Setup()
        {
            _myPlayerStatus = GetComponent<PlayerStatus>();
            _myPlayerMove = GetComponent<TempPlayerMove>();
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }

            //�X�^�~�i�̏���x��2�{��
            _myPlayerMove.Pulsation(true);

            //�o���Ŏ󂯂�_���[�W��2�{��
            _myPlayerStatus.PulsationBleeding(true);

            //���������͂�����
            //Todo�F�����������

            Debug.Log("�S��������");
        }

        public void Hide()
        {
            _myPlayerMove.Pulsation(false);
            _myPlayerStatus.PulsationBleeding(false);
        }
    }

}