using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Fruit : MonoBehaviour
{
    [SerializeField] protected float forwardSpeed;    
    [SerializeField] protected int damageAmount;
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected Rigidbody selfRB;
    protected bool isFired = false;

    void Start()
    {
        if(selfRB==null)
        {
            selfRB = this.GetComponent<Rigidbody>();
        }
    }
    
    void Update()
    {
        // if(isFired)
        // {
            selfRB.velocity = transform.forward * forwardSpeed;
        //}
        Debug.Log(selfRB.velocity);
    }

    void Fire()
    {
        selfRB.useGravity=true;
        isFired=true;
    }
}
