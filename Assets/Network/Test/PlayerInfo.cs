using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerInfo : NetworkBehaviour
{
    [Networked] public int debugNumber { get; set; } = 0;
}
