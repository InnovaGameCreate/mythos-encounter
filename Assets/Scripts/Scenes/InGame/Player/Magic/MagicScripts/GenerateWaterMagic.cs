using Scenes.Ingame.Enemy;
using System.Collections;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// ���𐶐��������
    /// </summary>
    public class GenerateWaterMagic : Magic
    {
        private EnemyStatus[] _enemyStatuses;
        private GameObject _waterEffect;

        public override void ChangeFieldValue()
        {
            chantTime = 20f;
            consumeSanValue = 10;
            Debug.Log("�������Ă���������FGenerateWaterMagic" + "\n�������Ă�����̉r�����ԁF" + chantTime + "\n�������Ă������SAN�l����ʁF" + consumeSanValue);
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
                    //Todo�F���G�t�F�N�g����
                    //_waterEffect = Instantiate(Resources.Load<GameObject>(���G�t�F�N�g�̃p�X��), myPlayerStatus.gameObject.transform.position, Quaternion.identity, myPlayerStatus.gameObject.transform);

                    _enemyStatuses = FindObjectsOfType<EnemyStatus>();
                    for (int i = 0; i < _enemyStatuses.Length; i++)
                    {
                        _enemyStatuses[i].SetCheckWaterEffect(true);
                    }
                    StartCoroutine(CancelWaterEffect());

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

        private IEnumerator CancelWaterEffect()
        {
            yield return new WaitForSeconds(60f);
            Destroy(_waterEffect);
            Debug.Log("���̉e���������Ȃ�܂��B");
            for (int i = 0; i < _enemyStatuses.Length; i++)
            {
                _enemyStatuses[i].SetCheckWaterEffect(false);
            }
        }
    }
}