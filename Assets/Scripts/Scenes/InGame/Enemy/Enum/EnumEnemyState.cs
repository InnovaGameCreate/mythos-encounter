using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public enum EnemyState
    {
        /// <summary>
        /// �Ȃɂ��o�^����Ă��Ȃ����
        /// </summary>
        None,
        /// <summary>
        /// ����
        /// </summary>
        Patorolling,
        /// <summary>
        /// ���G
        /// </summary>
        Searching,
        /// <summary>
        /// �ǐ�
        /// </summary>
        Chese,
        /// <summary>
        /// �U��
        /// </summary>
        Attack,
        /// <summary>
        /// �ގU
        /// </summary>
        FallBack,
        /// <summary>
        /// ����s��
        /// </summary>
        Special
    }


}
