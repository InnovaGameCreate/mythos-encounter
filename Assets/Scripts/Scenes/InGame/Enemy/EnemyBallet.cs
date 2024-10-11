using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Fusion;

public class EnemyBallet : NetworkBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private int _breedDamage;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField][Tooltip("�������A1�Ȃ�K��")] private float _accuracy;
    [SerializeField][Tooltip("�������Ȃ������ꍇ�A�Ώۂ���ǂꂾ���̋��������炵�Ēe������Ă����邩")] private float _shootingErrorDistance;
    [SerializeField][Tooltip("�^�[�Q�b�g���W�ɑ΂���␳�A�v���C���[���W���n�ʂɐG��Ă��邱�ƂɑΉ�")]private Vector3 _hitPositionSetting;
    
    private PlayerStatus _targetStatus;
    private NetworkObject _targetNetworkObject;
    private GameObject _targetObject;
   
    private Vector3 _shootingErrorVector;
    private bool _overShoot;//����𒴂�����

    [Networked] private bool _stop { get; set; }//��~����
    [Networked] private bool _hit { get; set; } = false;//�������邩�ǂ���

    public void Init(NetworkId targetId)
    {
        Runner.TryFindObject(targetId ,out _targetNetworkObject);
        _targetObject = _targetNetworkObject.gameObject;
        _targetStatus = _targetObject.GetComponent<PlayerStatus>();
        if (HasStateAuthority) {//�ǂ̂悤�ȏ����ŉ����N���邩���߂�̂̓z�X�g�i�Ȃ�Init�̓z�X�g�ł����Ă΂�Ȃ��j
            if (UnityEngine.Random.RandomRange(0f, 1f) <= _accuracy)
            {
                _hit = true;
            }
            this.transform.rotation = Quaternion.LookRotation((_targetObject.transform.position + _hitPositionSetting + _shootingErrorVector - this.transform.position), Vector3.up);
            _shootingErrorVector = new Vector3(0, 0, 0);
            if (!_hit)
            {
                float _targetAngleY;
                _targetAngleY = Mathf.Atan2(_targetObject.transform.position.x + _hitPositionSetting.x - this.transform.position.x, _targetObject.transform.position.z + _hitPositionSetting.z - this.transform.position.z);
                if (UnityEngine.Random.RandomRange(0, 2) == 0)//���E�ǂ���ɂ��炷��
                {
                    _targetAngleY += Mathf.PI / 4;
                }
                else
                {
                    _targetAngleY += Mathf.PI / -4;
                }
                _shootingErrorVector.x = _shootingErrorDistance * Mathf.Cos(_targetAngleY);
                _shootingErrorVector.z = _shootingErrorDistance * Mathf.Sin(_targetAngleY);
                if (UnityEngine.Random.RandomRange(0, 2) == 0)//�㉺�ǂ���ɂ��炷��
                {
                    _shootingErrorVector.z = _shootingErrorDistance;
                }
                else
                {
                    _shootingErrorVector.z = -_shootingErrorDistance;
                }
                this.transform.rotation = Quaternion.LookRotation((_targetObject.transform.position + _hitPositionSetting + _shootingErrorVector - this.transform.position), Vector3.up);
            }
        }



    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {

        if (!_stop)
        {
            transform.position += transform.forward * _speed * Runner.DeltaTime;
            if (HasStateAuthority) //�O�i�ȊO�̓��ꏈ���̓z�X�g���s��
            {
                if (!_overShoot)
                {//����̍��W�𒴂��Ă��Ȃ��̂ł����
                    this.transform.rotation = Quaternion.LookRotation((_targetObject.transform.position + _hitPositionSetting + _shootingErrorVector - this.transform.position), Vector3.up);
                    if ((this.transform.position - _targetObject.transform.position - _hitPositionSetting).magnitude < _speed * Runner.DeltaTime)
                    {//���t���[���ŖڕW�n�_(�G�̈ʒu+�덷)�֓��B����ꍇ
                        _overShoot = true;
                        if (_hit)
                        { //��������ꍇ
                            this.transform.position += this.transform.forward * (this.transform.position - _targetObject.transform.position - _hitPositionSetting).magnitude;
                            this.transform.parent = _targetObject.transform;
                            _stop = true;

                            //�_���[�W�̏���
                            _targetStatus.ChangeHealth(_damage, ChangeValueMode.Damage);
                            _targetStatus.OnEnemyAttackedMeEvent.OnNext(Unit.Default);
                        }
                    }
                }
            }           
        }
        _lifeTime -= Time.deltaTime;
        
        if (_lifeTime < 0) {
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!_hit)
        { //�ǂȂǂɂԂ�����
            if (!other.CompareTag("Enemy")) {
                _stop = true;
            }
        }
    }

}
