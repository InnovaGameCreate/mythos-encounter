using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Network
{
    /// <summary>
    /// プレイヤーオブジェクトにアタッチする
    /// </summary>
    public class ConnectionToken : NetworkBehaviour
    {
        [Networked] public int token { get; set; }
    }
}
