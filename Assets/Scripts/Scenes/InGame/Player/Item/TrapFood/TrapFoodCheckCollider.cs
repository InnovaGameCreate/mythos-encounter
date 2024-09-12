using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFoodCheckCollider : MonoBehaviour
{
    private bool _isTriggered = false;
    public bool IsTriggered { get { return _isTriggered; } }
    private void OnTriggerEnter(Collider other)
    {
        _isTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _isTriggered = false;
    }
}
