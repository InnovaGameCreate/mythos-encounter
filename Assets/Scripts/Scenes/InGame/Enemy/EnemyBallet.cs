using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBallet : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.LookAt(GameObject.Find("Player").transform.position);
        _rigidbody.velocity = transform.forward * _speed;
        Debug.Log("����");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _lifeTime -= Time.deltaTime;
        
        if (_lifeTime < 0) { Destroy(this.gameObject); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") { //�������I
            collision.gameObject.GetComponent<Scenes.Ingame.Player.PlayerStatus>().ChangeHealth(_damage, "Damage");
            Destroy(this.gameObject);
        } else if (collision.gameObject.tag == "Stage") {
            Destroy(this.gameObject);
        }
        
    }
}
