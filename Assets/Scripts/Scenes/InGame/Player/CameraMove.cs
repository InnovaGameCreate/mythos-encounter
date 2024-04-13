using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;


namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �J�����i�v���C���[�̎��_�j��ύX����N���X
    /// player�ɂ��Ă���J�����ɃA�^�b�`����
    /// </summary>
    public class CameraMove : MonoBehaviour
    {
        //�J�����̗h��Ɋւ���ݒ�
        [SerializeField] private float _offSet;//�J������h�炷��
        private Vector3 _cameraPositionDefault;
        private Sequence sequence;
        //�R���|�[�l���g�̎擾
        //[SerializeField] private PlayerStatus _myPlayerState; 

        private void Start()
        {
            //�����̏����̏ꏊ���L�^
            _cameraPositionDefault = this.transform.position;
        }

        /// <summary>
        /// �N���b�v�̎��Ԃ̒�������ɁA�J�����ړ��̎��������肷��B
        /// ���ӁF�����̃N���b�v�͊�{�����Q��̃Z�b�g�ɂȂ��Ă���B�P�񕪂ɂ���ɂ�clipTime / 2���g������
        /// </summary>
        /// <param name="clipTime"></param>
        public void ChangeViewPoint(float clipTime)
        {
            //�����Ȃ���Ԃ̎��͈�����0�����Ă���
            //�ҋ@��Ԃł͂ق�̏��������������Ǝ��_���ς��
            if (clipTime == 0)
            {
                var cycle = 2.0f;//���_�ړ��̎���

                sequence.Kill();
                sequence = DOTween.Sequence();
                sequence.Append(this.transform.DOLocalMove(new Vector3(0, _cameraPositionDefault.y - _offSet / 2, 0), cycle / 2));
                sequence.Append(this.transform.DOLocalMove(new Vector3(0, _cameraPositionDefault.y, 0), cycle / 2));
                sequence.Play().SetLoops(-1, LoopType.Yoyo);

                return;
            }
            

            //����ݒ肷�鋓�����쐬
            sequence.Kill();
            sequence = DOTween.Sequence();
            sequence.Append(this.transform.DOLocalMove(new Vector3(0, _cameraPositionDefault.y - _offSet, 0), clipTime / 2));
            sequence.Append(this.transform.DOLocalMove(new Vector3(0, _cameraPositionDefault.y, 0), clipTime / 2));
            sequence.Play().SetLoops(-1, LoopType.Yoyo);     
        }
    }
}
    
