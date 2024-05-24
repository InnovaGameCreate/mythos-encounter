using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// �v���C���[�̖��@�֘A���Ǘ�����X�N���v�g
    /// </summary>
    public class PlayerMagic : MonoBehaviour
    {
        private bool _isCanUseMagic = true;//���ݖ��@���g���邩�ۂ�
        [SerializeField] private Magic _myMagic;//�g�p�\�Ȗ��@
        public void Start()
        {
            //���g��PlayerStatus���擾
            PlayerStatus myPlayerStatus = this.GetComponent<PlayerStatus>();

            //_myMagic�̒��g�����g���ݒ肵�������ɐݒ肷�鏈��
            //���łł͖���(�C���Q�[���O���������ꂽ�����)

            //�����X�N���v�g��PlayerStatus��PlayerMagic���擾������
            _myMagic.myPlayerStatus = myPlayerStatus;
            _myMagic.myPlayerMagic = this;

            //Q�L�[�Ŏ����̉r�����J�n or ���~�����鏈��
            this.UpdateAsObservable()
                .Where(_ => _isCanUseMagic && Input.GetKeyDown(KeyCode.Q))
                .ThrottleFirst(TimeSpan.FromMilliseconds(100))
                .Subscribe(_ =>
                {
                    if (myPlayerStatus.nowPlayerUseMagic)//�������r�����Ă�����
                    {
                        //�r�����̈ړ����x50%Down������
                        myPlayerStatus.UseMagic(false);

                        //���@���g���������L�����Z��
                        _myMagic.cancelMagic = true;
                        Debug.Log("����ɂ��r�����~");
                    }
                    else//�������܂��r�����Ă��Ȃ��Ƃ�
                    {
                        //San�l��10�ȉ��̂Ƃ��͉r���ł��Ȃ�
                        if (myPlayerStatus.nowPlayerSanValue <= 10)
                        {
                            Debug.Log("SAN�l��10�ȉ��Ȃ̂ŉr���ł��܂���");
                            return;
                        }

                        //�e�����ňꕔ�g�p���Ȃ��ėǂ��󋵂ł���Ύ������g�킹�Ȃ�
                        bool needMagic = true;//�������g���K�v�����邩�ۂ�
                        switch (_myMagic)
                        {
                            case SelfBrainwashMagic:
                                if (myPlayerStatus.nowPlayerSanValue > 50)
                                {
                                    needMagic = false;
                                    Debug.Log("�������g���K�v������܂���");
                                }
                                break;
                            default: 
                                break;
                        }

                        if (needMagic)
                        {
                            //�r�����͈ړ����x50%Down
                            myPlayerStatus.UseMagic(true);

                            //���@���g������
                            _myMagic.MagicEffect();
                            Debug.Log("�����̉r���J�n");
                        }
                    }                   
                });

            //�U����������Ƃ�������Bool��True�ɂȂ����Ƃ��Ɏ����r���𒆒f
            myPlayerStatus.OnEnemyAttackedMe
                .Where(_ => _isCanUseMagic)
                .Subscribe(_ =>
                {
                    //�r�����̈ړ����x50%Down������
                    myPlayerStatus.UseMagic(false);

                    //���@���g���������L�����Z��
                    _myMagic.cancelMagic = true;
                    Debug.Log("�U�����󂯂��̂ŉr�����~�I");
                }).AddTo(this);
        }

        public void ChangeCanUseMagicBool(bool value)
        {
            _isCanUseMagic = false;
        }
    }
}

