using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;

namespace Scenes.Ingame.Player
{
    public class PlayerAnimationManager : NetworkBehaviour
    {
        [SerializeField]
        SimpleKCC _simpleKCC;
        [SerializeField]
        Animator _animator;

        public override void Render()
        {
            var moveVelocity = GetAnimationMoveVelocity();

            //Debug.Log("MoveX:" + moveVelocity.x);
            //Debug.Log("MoveZ:" + moveVelocity.z);

            //_animator.SetFloat("MovementSpeed", moveVelocity);
            _animator.SetFloat("MoveZ", moveVelocity.z, 0.05f, Time.deltaTime);
            _animator.SetFloat("MoveX", moveVelocity.x, 0.05f, Time.deltaTime);
        }


        Vector3 GetAnimationMoveVelocity()
        {
            if(_simpleKCC.RealSpeed < 0.01f)
                return default;

            var velocity = _simpleKCC.RealVelocity;
            //Debug.Log(velocity);
            velocity.y = 0f;

            //1に正規化
            /*if(velocity.sqrMagnitude > 1f)
            {
                velocity.Normalize();
            }*/

            return transform.InverseTransformVector(velocity);  //ワールド空間からローカル空間へ変換
        }
    }
}

