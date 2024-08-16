using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Scenes.Ingame.Player
{
    public class RecoverMagic : Magic
    {
        [SerializeField] private GameObject _effect;
        public override void ChangeFieldValue()
        {
            chantTime = 15f;
            consumeSanValue = 10;
            Debug.Log("�������Ă���������FRecoverMagic" + "\n�������Ă�����̉r�����ԁF" + chantTime + "\n�������Ă������SAN�l����ʁF" + consumeSanValue);
        }

        public override void MagicEffect()
        {
            startTime = Time.time;
            StartCoroutine(Magic());
        }

        private IEnumerator Magic()
        {
            while (true)
            {
                yield return null;

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
                    myPlayerStatus.ChangeHealth(0.5f, "Heal");
                    GameObject effect = Instantiate(Resources.Load<GameObject>("Effect/RecoverEffect/RecoverEffect"), myPlayerStatus.gameObject.transform.position, Quaternion.identity, myPlayerStatus.gameObject.transform);
                    Destroy(effect, effect.GetComponent<VisualEffect>().GetFloat("LifeTime"));

                    //SAN�l����
                    myPlayerStatus.ChangeSanValue(consumeSanValue, "Damage");

                    //�������g���Ȃ��悤�ɂ���
                    myPlayerMagic.ChangeCanUseMagicBool(false);

                    //���������r���̏I����ʒm
                    myPlayerMagic.OnPlayerFinishUseMagic.OnNext(default);
                    yield break;
                }
            }
        }
    }

}