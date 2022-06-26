using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watermelon : Fruit
{

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;


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
        if(!collision.gameObject.CompareTag("Player") 
            && !collision.gameObject.CompareTag("Projectile"))
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            Rigidbody rb =collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(
                    explosionForce, transform.position, explosionRadius);
            }

            if (collision.gameObject.TryGetComponent(out IDamageable dmgInterface))
            {
                dmgInterface.TakeDamage(damageAmount);
            }
            //apply force to nearby rigidbodies with colliders and damage to IDamageables
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {
                Rigidbody rb1 = nearbyObject.GetComponent<Rigidbody>();
                if (rb1 != null)
                {
                    rb1.AddExplosionForce(
                        explosionForce, transform.position, explosionRadius);
                }

                if (nearbyObject.TryGetComponent(out IDamageable dmgInt))
                {
                    dmgInt.TakeDamage(damageAmount);
                }
            }
            
            Destroy(gameObject);
        }
    }
}
