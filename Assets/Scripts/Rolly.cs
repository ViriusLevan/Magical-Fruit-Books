using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rolly : Robot
{
    private List<Vector3> directions;
    private int currentDir;
    [SerializeField]private int damage;

    // Start is called before the first frame update
     protected override void Start()
    {
        directions = new List<Vector3>(){
            Vector3.forward
            ,new Vector3(1,0,1)
            ,Vector3.right
            ,new Vector3(1,0,-1)
            ,Vector3.back
            ,new Vector3(-1,0,-1)
            ,Vector3.left
            ,new Vector3(-1,0,1)
        };
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    void FixedUpdate()
    {
        if(selfRB.velocity.magnitude<movespeed)
        {
            selfRB.AddForce(accelForce*directions[currentDir]);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(!col.gameObject.CompareTag("Ground"))
        {
            int rng=0;
            do{
               rng = Random.Range(0,8);
            }while(rng==currentDir);
            currentDir=rng;
        }
        if(col.gameObject.CompareTag("Player"))
        {
            //Have player take damage
            col.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

}
