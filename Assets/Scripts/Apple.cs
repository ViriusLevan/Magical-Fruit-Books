using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Fruit
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        selfRB.velocity = transform.forward * forwardSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out IDamageable dmgInterface))
            {
                dmgInterface.TakeDamage(damageAmount);
            }

            AudioSource.PlayClipAtPoint(base.hitSound,transform.position);
            Destroy(gameObject);
        }
    }
}
