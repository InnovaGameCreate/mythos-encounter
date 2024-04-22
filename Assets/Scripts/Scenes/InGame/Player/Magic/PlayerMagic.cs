using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

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

            //�������g������
            this.UpdateAsObservable()
                .Where(_ => _isCanUseMagic == true && Input.GetKeyDown(KeyCode.Q))
                .Subscribe(_ =>
                {
                    //���@���g��������SAN�l��������
                    _myMagic.MagicEffect();
                    myPlayerStatus.ChangeSanValue(_myMagic.consumeSanValue, "Damage");

                    //�N�[���^�C���J�n
                    StartCoroutine(MagicCoolTime(_myMagic.coolTime));
                });
        }


        private IEnumerator MagicCoolTime(float coolTime)
        {
            _isCanUseMagic = false;
            yield return new WaitForSeconds(coolTime);
            _isCanUseMagic = true;
            Debug.Log("�����N�[���^�C���I��");
        }
    }
}

