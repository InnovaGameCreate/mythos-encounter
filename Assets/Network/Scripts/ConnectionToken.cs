using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Network
{
    /// <summary>
    /// �v���C���[�I�u�W�F�N�g�ɃA�^�b�`����
    /// </summary>
    public class ConnectionToken : NetworkBehaviour
    {
        [Networked] public int token { get; set; }
    }
}
