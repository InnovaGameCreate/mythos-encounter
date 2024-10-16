using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Burst.CompilerServices;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 視界にある１タイル以内の場所に障壁を作成する呪文
    /// </summary>
    public class CreateBarricadeMagic : Magic
    {
        private readonly float _tileLength = 5.85f;//１タイルの長さ
        private readonly float _wallLength = 5.8f;//壁の高さ
        private GameObject _mainCamera;
        private GameObject _barricadePrefab;
        [SerializeField] private GameObject _CreatedBarricade;
        [SerializeField] private bool isCanCreate = false;

        //デバック関連の変数
        [Header("デバック関連")]
        private bool _debugMode = false;
        [SerializeField] private Vector3 _leftPositoon , _rightPosition;

        public override void ChangeFieldValue()
        {
            chantTime = 5f;
            consumeSanValue = 10;
            Debug.Log("装備している呪文名：CreateBarricadeMagic" + "\n装備してる呪文の詠唱時間：" + chantTime + "\n装備してる呪文のSAN値消費量：" + consumeSanValue);
        }

        public override void MagicEffect()
        {
            startTime = Time.time;
            StartCoroutine(Magic());
        }

        private IEnumerator Magic()
        {
            _mainCamera = GetComponentInChildren<Camera>().gameObject;
            _barricadePrefab = (GameObject)Resources.Load("Prefab/Magic/Barricade");

            RaycastHit hit;
            RaycastHit leftHit, rightHit;
            int defaultlayerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Wall");//主要なLayer以外・壁に反応するようにする
            int floorlayerMask = LayerMask.GetMask("Floor");//床にだけ反応するようにする

            while (true)
            {
                yield return null;

                if (_debugMode)
                    Debug.Log(Time.time - startTime);

                //床に向けてRayを飛ばす
                Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hit, _tileLength, floorlayerMask);
                Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * _tileLength, Color.black);

                //床にぶつかっていた時
                if (hit.collider != null)
                {
                    //着弾地点から左右にRayを飛ばす。Vector3.up * 0.1fはoffset(絨毯対策)
                    Physics.Raycast(hit.point + Vector3.up * 0.15f, this.transform.right * -1, out leftHit, Mathf.Infinity, defaultlayerMask);
                    Physics.Raycast(hit.point + Vector3.up * 0.15f, this.transform.right, out rightHit, Mathf.Infinity, defaultlayerMask);

                    Debug.DrawRay(hit.point + Vector3.up * 0.15f, this.transform.right * -100, Color.red);
                    Debug.DrawRay(hit.point + Vector3.up * 0.15f, this.transform.right * 100, Color.blue);

                    //左右に障害物があったとき
                    if (leftHit.collider != null && rightHit.collider != null)
                    {
                        Vector3 center = (leftHit.point + rightHit.point) / 2 + new Vector3(0, _wallLength, 0) / 2;
                        isCanCreate = true;

                        _leftPositoon = leftHit.point;
                        _rightPosition = rightHit.point;
                    
                        //プレビューの作成・移動
                        if (_CreatedBarricade == null)//プレビューがないときは作成
                        {
                            _CreatedBarricade = Instantiate(_barricadePrefab, center, _barricadePrefab.transform.rotation);
                        }
                        else//プレビューがあれば移動
                        {
                            _CreatedBarricade.transform.position = center;
                        }

                        //障壁の向きとサイズを調整
                        _CreatedBarricade.transform.rotation = this.gameObject.transform.rotation;

                        float distance = Vector3.Distance(leftHit.point, rightHit.point);


                        if(Mathf.Abs(leftHit.point.x - rightHit.point.x) >= _tileLength / 2)//z軸方向にPlayerが向いているとき
                            _CreatedBarricade.transform.localScale = new Vector3(distance, _wallLength, 1);
                        else//x軸方向にPlayerが向いているとき
                            _CreatedBarricade.transform.localScale = new Vector3(distance, _wallLength, 1);

                    }

                }
                else//床に最初のRayが当たっていなかったとき
                {
                    isCanCreate = false;
                    if (_CreatedBarricade != null)//プレビューが作成されていたら破壊
                    {
                        Destroy(_CreatedBarricade);
                    }
                }
                //攻撃を食らった際にこのコルーチンを破棄              
                if (cancelMagic == true)
                {
                    cancelMagic = false;
                    Destroy(_CreatedBarricade);
                    yield break;
                }

                //呪文発動
                //詠唱開始から5秒以上たっている & プレビューがある & 左クリック押す
                if (Time.time - startTime >= chantTime && isCanCreate && Input.GetMouseButton(0))
                {
                    Debug.Log("呪文発動！");
                    //効果発動
                    _CreatedBarricade.GetComponent<BoxCollider>().enabled = true;


                    //SAN値減少
                    myPlayerStatus.ChangeSanValue(consumeSanValue, ChangeValueMode.Damage);

                    //呪文を使えないようにする
                    myPlayerMagic.ChangeCanUseMagicBool(false);

                    //成功した詠唱の終了を通知
                    myPlayerMagic.OnPlayerFinishUseMagic.OnNext(default);
                    yield break;
                }
            }
        }

        
    }
}
