using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Network
{
    /// <summary>
    /// �l�b�g���[�N�I�u�W�F�N�g�ɃA�^�b�`����
    /// </summary>
    public class ObjectToken : NetworkBehaviour
    {
        [Networked] public string token { get; set; }
        [Networked] public Vector3 position { get; set; }
        [Networked] public Quaternion rotation { get; set; }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer) return;

            position = transform.position;
            rotation = transform.rotation;
        }
    }
}
