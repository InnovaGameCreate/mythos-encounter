using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Burst.CompilerServices;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// ���E�ɂ���P�^�C���ȓ��̏ꏊ�ɏ�ǂ��쐬�������
    /// </summary>
    public class CreateBarricadeMagic : Magic
    {
        private bool _debugMode = false;
        private readonly float _tileLength = 5.85f;//�P�^�C���̒���
        private readonly float _wallLength = 5.8f;//�ǂ̍���
        private GameObject _mainCamera;
        private GameObject _barricadePrefab;
        [SerializeField] private GameObject _CreatedBarricade;
        [SerializeField] private bool isCanCreate = false;

        public override void ChangeFieldValue()
        {
            chantTime = 105f;
            consumeSanValue = 10;
            Debug.Log("�������Ă���������FCreateBarricadeMagic" + "\n�������Ă�����̉r�����ԁF" + chantTime + "\n�������Ă������SAN�l����ʁF" + consumeSanValue);
        }

        public override void MagicEffect()
        {
            startTime = Time.time;
            StartCoroutine(Magic());
        }

        private IEnumerator Magic()
        {
            _mainCamera = GetComponentInChildren<Camera>().gameObject;
            _barricadePrefab = (GameObject)Resources.Load("Prefab/Barricade");

            RaycastHit hit;
            RaycastHit leftHit, rightHit;
            int defaultlayerMask = LayerMask.GetMask("Default");//��v��Layer�ȊO�ɔ�������悤�ɂ���
            int floorlayerMask = LayerMask.GetMask("Floor");//���ɂ�����������悤�ɂ���

            while (true)
            {
                yield return null;

                if (_debugMode)
                    Debug.Log(Time.time - startTime);

                //���Ɍ�����Ray���΂�
                Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _tileLength, floorlayerMask);
                Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * _tileLength, Color.black);

                //���ɂԂ����Ă�����
                if (hit.collider != null)
                {
                    //���e�n�_���獶�E��Ray���΂��BVector3.up * 0.1f��offset
                    Physics.Raycast(hit.point + Vector3.up * 0.1f, this.transform.right * -1, out leftHit, Mathf.Infinity, defaultlayerMask);
                    Physics.Raycast(hit.point + Vector3.up * 0.1f, this.transform.right, out rightHit, Mathf.Infinity, defaultlayerMask);

                    Debug.DrawRay(hit.point, this.transform.right * -100, Color.red);
                    Debug.DrawRay(hit.point, this.transform.right * 100, Color.blue);

                    //���E�ɏ�Q�����������Ƃ�
                    if (leftHit.collider != null && rightHit.collider != null)
                    {
                        Vector3 center = (leftHit.point + rightHit.point) / 2 + new Vector3(0, _wallLength, 0) / 2;
                        isCanCreate = true;
                    
                        //�v���r���[�̍쐬�E�ړ�
                        if (_CreatedBarricade == null)//�v���r���[���Ȃ��Ƃ��͍쐬
                        {
                            _CreatedBarricade = Instantiate(_barricadePrefab, center, _barricadePrefab.transform.rotation);
                        }
                        else//�v���r���[������Έړ�
                        {
                            _CreatedBarricade.transform.position = center;
                        }

                        //��ǂ̌����ƃT�C�Y�𒲐�
                        _CreatedBarricade.transform.rotation = this.gameObject.transform.rotation;

                        if(Mathf.Abs(leftHit.point.x - rightHit.point.x) >= 1)//z��������Player�������Ă���Ƃ�
                            _CreatedBarricade.transform.localScale = new Vector3(Mathf.Abs(leftHit.point.x - rightHit.point.x), _wallLength, 1);
                        else//x��������Player�������Ă���Ƃ�
                            _CreatedBarricade.transform.localScale = new Vector3(Mathf.Abs(leftHit.point.z - rightHit.point.z), _wallLength, 1);

                    }

                }
                else//���ɍŏ���Ray���������Ă��Ȃ������Ƃ�
                {
                    isCanCreate = false;
                    if (_CreatedBarricade != null)//�v���r���[���쐬����Ă�����j��
                    {
                        Destroy(_CreatedBarricade);
                    }
                }
                //�U����H������ۂɂ��̃R���[�`����j��              
                if (cancelMagic == true)
                {
                    cancelMagic = false;
                    yield break;
                }

                //��������
                if (Time.time - startTime >= chantTime && isCanCreate)
                {
                    Debug.Log("���������I");
                    //���ʔ���
                    _CreatedBarricade.GetComponent<BoxCollider>().enabled = true;


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
