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
        [SerializeField]
        private bool _debugMode;

        //���G�s���̃N���X�ł�
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_myVisivilityMap != null) {
                Debug.LogError("�}�b�v��񂪂���܂���A_myVisivilityMap���쐬���Ă�������");
                return; }//���G�̏������ł��Ă��Ȃ��ꍇ


        }

        public void SetVisivilityMap(EnemyVisibilityMap setVisivilityMap) 
        { 
            _myVisivilityMap = setVisivilityMap;
        }
    }
}
