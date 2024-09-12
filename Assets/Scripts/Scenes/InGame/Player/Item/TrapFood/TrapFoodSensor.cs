using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFoodSensor : MonoBehaviour
{
    private bool _isActiveSensor = true;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Enemy"))
        {

            Vector3 vector = collider.transform.position - this.transform.position;
            int layerMask = ~(1 << LayerMask.NameToLayer("Enemy"));

            if (!Physics.Raycast(this.transform.position, vector, vector.magnitude, layerMask) && _isActiveSensor)
            {
                float random = Random.value * 100f;
                if (random <= 30f)
                {
                    MoveEnemy();
                    transform.parent.gameObject.layer = 0;
                    Destroy(this.gameObject);
                }
                _isActiveSensor = false;
                Invoke("ActiveSensor", 1f);
            }
        }
    }

    /// <summary>
    ///“GƒLƒƒƒ‰‚ğ—U“±‚·‚é‚½‚ß‚ÌŠÖ”
    /// </summary>
    private void MoveEnemy()
    {

    }

    private void ActiveSensor()
    {
        _isActiveSensor = true;
    }
}
