using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylin : Robot
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float firingCooldown=2f;
    [SerializeField] private float maxFiringDist = 30f;
    private float firingCountdown=0f;
    // Start is called before the first frame update
    void Start()
    {
        if(target==null){
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(firingCountdown>0)
        {
            firingCountdown-=Time.deltaTime;
        }
        TurnToTarget();
        CheckDistance();
    }

    void FixedUpdate()
    {
    }

    private void TurnToTarget()
    {
        //Rotating forward transform to direction of the player
        Vector3 direction = target.position - selfRB.position;
        direction.Normalize();
        Vector3 rotateToPlayer = Vector3.Cross(transform.forward, direction);
        rotateToPlayer = Vector3.Project(rotateToPlayer, transform.up);
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnRate*Time.deltaTime);
    }

    private void MoveForward()
    {
        if(selfRB.velocity.magnitude<movespeed)
        {
            selfRB.AddForce(accelForce*transform.forward);
        }
    }

    private void CheckDistance()
    {
        float dist = Vector3.Distance(target.position, transform.position);
        if(dist<=maxFiringDist)
        {
            CheckAngle();
        }
        else
        {
            MoveForward();
        }
    }

    private void CheckAngle()
    {
        float forwardAngle = Vector3.Angle(transform.forward, target.position);
        if(forwardAngle<=45f && firingCountdown<=0)
        {
            Fire();
            firingCountdown = firingCooldown;
        }
    }
    private void Fire()
    {
        Instantiate(bullet, firingPoint.position, firingPoint.rotation);
    }
    
}
