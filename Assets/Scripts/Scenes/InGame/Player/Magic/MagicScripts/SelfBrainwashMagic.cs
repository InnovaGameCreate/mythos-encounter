using Scenes.Ingame.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// ���g�ɐ��]�������Ĉꎞ�I�ɐ��C�ɖ߂�C��Ԉُ�𖳌����������
    /// ���ʎ��� 60�b
    /// </summary>
    public class SelfBrainwashMagic : Magic
    {
        private PlayerInsanityManager _myPlayerInsanityManager;
        private bool _debugMode = false;
        public override void ChangeFieldValue()
        {
            chantTime = 20f;
            consumeSanValue = 10;
            Debug.Log("�������Ă���������FSelfBrainwashMagic" + "\n�������Ă�����̉r�����ԁF" + chantTime + "\n�������Ă������SAN�l����ʁF" + consumeSanValue);
        }

        public override void MagicEffect()
        {
            startTime = Time.time;
            StartCoroutine(Magic());
        }

        private IEnumerator Magic()
        {
            _myPlayerInsanityManager = this.GetComponent<PlayerInsanityManager>();
            while (true)
            {
                yield return null;

                if(_debugMode)
                    Debug.Log(Time.time - startTime);

                //�U����H������ۂɂ��̃R���[�`����j��              
                if (cancelMagic == true)
                {
                    cancelMagic = false;
                    yield break;
                }

                //��������
                if (Time.time - startTime >= chantTime)
                {
                    Debug.Log("���������I");
                    //���ʔ���
                    StartCoroutine(_myPlayerInsanityManager.SelfBrainwash());

                    //SAN�l����
                    myPlayerStatus.ChangeSanValue(consumeSanValue, "Damage");

                    //�������g���Ȃ��悤�ɂ���
                    myPlayerMagic.ChangeCanUseMagicBool(false);
                    yield break;
                }
            }
        }
    }
}
