using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody selfRB;
    [SerializeField] private float forwardSpeed;
    [SerializeField] private int damage;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        selfRB.velocity = transform.forward * forwardSpeed;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            //Have player take damage
            col.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
