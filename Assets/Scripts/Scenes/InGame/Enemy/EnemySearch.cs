using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class EnemySearch : MonoBehaviour//�p�g���[������B�v���C���[�̍��Ղ�T���B�����Ԃƍ��G��Ԃ̓��������肵�A�ǐՂƍU����Ԃւ̈ڍs���s���B
    {
        private EnemyVisibilityMap _myVisivilityMap;
        [SerializeField]
        private float _checkRate;//���b���ƂɎ��E�̏�Ԃ��`�F�b�N���邩
        private float _checkTimeCount;//�O��`�F�b�N���Ă���̎��Ԃ��v��
        [SerializeField]
        private bool _debugMode;
        [SerializeField]
        private EnemyMove _myEneyMove;

        [SerializeField]
        private float _visivilityRange;//���Ƃł���̓X�e�[�^�X�Ɏ��������

        //���G�s���̃N���X�ł�
        // Start is called before the first frame update
        void Start()
        {
        }

        void FixedUpdate()
        {
            if (_myVisivilityMap != null) {
                Debug.LogError("�}�b�v��񂪂���܂���A_myVisivilityMap���쐬���Ă�������");
                return; }//���G�̏������ł��Ă��Ȃ��ꍇ
            if (true) { //�����Ԃ̏ꍇ
                if (_myEneyMove.endMove)
                { //�ړ����I����Ă���ꍇ�V���Ȉړ�����擾����

                    //�ړ�����擾���郁�\�b�h����������
                    _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                }
                //���Ă����G���A���L�^���Ă䂭
                _checkTimeCount += Time.deltaTime;
                if (_checkTimeCount > _checkRate) {
                    _checkTimeCount = 0;
                    //�����Ɍ��݈ʒu�ƌ������̏������݂��˗����郁�\�b�h������
                    _myVisivilityMap.CheckVisivility(this.transform.position,_visivilityRange);

                }
            
            
            
            
            } 
        }

        public void SetVisivilityMap(EnemyVisibilityMap setVisivilityMap) 
        { 
            _myVisivilityMap = setVisivilityMap;
        }
    }
}
