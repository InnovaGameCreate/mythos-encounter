using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerMove : MonoBehaviour
{
    CharacterController _characterController;
    Vector3 _moveVelocity;
    private float moveSpeed = 5;
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {

        float moveMouseX = Input.GetAxis("Mouse X");
        if (Mathf.Abs(moveMouseX) > 0.001f)
        {
            // âÒì]é≤ÇÕÉèÅ[ÉãÉhç¿ïWÇÃYé≤
            transform.RotateAround(transform.position, Vector3.up, moveMouseX);
        }

        _moveVelocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            _moveVelocity += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _moveVelocity -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _moveVelocity -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _moveVelocity += transform.right;
        }
        _moveVelocity = _moveVelocity.normalized;
        _characterController.Move(_moveVelocity * Time.deltaTime * moveSpeed);
    }
}
