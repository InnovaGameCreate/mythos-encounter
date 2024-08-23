using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using DG.Tweening;
using TMPro;
using UnityEditor.Rendering;

public class EnemyBallet : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private int _breedDamage;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField][Tooltip("命中率、1なら必中")] private float _accuracy;
    [SerializeField][Tooltip("命中しなかった場合、対象からどれだけの距離をずらして弾頭を飛翔させるか")] private float _shootingErrorDistance;
    [SerializeField][Tooltip("ターゲット座標に対する補正、プレイヤー座標が地面に触れていることに対応")]private Vector3 _hitPositionSetting;
    
    private PlayerStatus _targetStatus;
    private GameObject _target;
    private bool _hit = false;//命中するかどうか
    private Vector3 _shootingErrorVector;
    private bool _overShoot;//相手を超えたら
    private bool _stop;//停止する


    public void Init(PlayerStatus target)
    {
        _targetStatus = target;
        _target = _targetStatus.gameObject;
        if (UnityEngine.Random.RandomRange(0f,1f) <= _accuracy) { 
            _hit = true;
        }
        this.transform.rotation = Quaternion.LookRotation((_target.transform.position + _hitPositionSetting + _shootingErrorVector - this.transform.position), Vector3.up);
        _shootingErrorVector = new Vector3(0, 0, 0);
        if (!_hit)
        {
            float _targetAngleY;
            _targetAngleY = Mathf.Atan2(_target.transform.position.x + _hitPositionSetting.x - this.transform.position.x, _target.transform.position.z + _hitPositionSetting.z - this.transform.position.z);
            if (UnityEngine.Random.RandomRange(0, 2) == 0)//左右どちらにずらすか
            {
                _targetAngleY += Mathf.PI / 4;
            }
            else
            {
                _targetAngleY += Mathf.PI / -4;
            }
            _shootingErrorVector.x = _shootingErrorDistance * Mathf.Cos(_targetAngleY);
            _shootingErrorVector.z = _shootingErrorDistance * Mathf.Sin(_targetAngleY);
            if (UnityEngine.Random.RandomRange(0, 2) == 0)//上下どちらにずらすか
            {
                _shootingErrorVector.z = _shootingErrorDistance; 
            }
            else
            {
                _shootingErrorVector.z = -_shootingErrorDistance;
            }
            this.transform.rotation = Quaternion.LookRotation((_target.transform.position + _hitPositionSetting + _shootingErrorVector - this.transform.position), Vector3.up);
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
            
        if (!_stop)
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
            if (!_overShoot)
            {//相手の座標を超えていないのであれば
                this.transform.rotation = Quaternion.LookRotation((_target.transform.position + _hitPositionSetting + _shootingErrorVector - this.transform.position), Vector3.up);
                if ((this.transform.position - _target.transform.position - _hitPositionSetting).magnitude < _speed * Time.deltaTime)
                {//次フレームで目標地点(敵の位置+誤差)へ到達する場合
                    _overShoot = true;
                    if (_hit) { //命中する場合
                        this.transform.position += this.transform.forward * (this.transform.position - _target.transform.position - _hitPositionSetting).magnitude;
                        this.transform.parent = _target.transform;
                        _stop = true;

                        //ダメージの処理
                        _targetStatus.ChangeHealth(_damage, "Damage");
                        _targetStatus.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
                    }
                }
            }
            
        }
        _lifeTime -= Time.deltaTime;
        
        if (_lifeTime < 0) { Destroy(this.gameObject); }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!_hit)
        { //壁などにぶつかった
            if (!other.CompareTag("Enemy")) {
                _stop = true;
            }
        }
    }

}
