using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaObjCylinder : MonoBehaviour
{
    private float upwardForce=10f,downwardForce=5f,objectHeight=2f,horizontalForce=10f;
    private bool goDown=true,goForward;
    private float speedRNG;
    private float originZ;

    void Start()
    {
        originZ = transform.position.z;
        speedRNG=UnityEngine.Random.Range(0,6);
        goForward = UnityEngine.Random.Range(0,2)>0 ? true : false;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y<-1f*objectHeight)
        {
            goDown=false;
            speedRNG=UnityEngine.Random.Range(0,11);
        }
        else if(transform.position.y>=objectHeight)
        {            
            goDown=true;
            speedRNG=UnityEngine.Random.Range(0,11);
        }

        if(transform.position.z>originZ+15)
        {
            goForward=false;
        }
        else if(transform.position.z<originZ-15)
        {
            goForward=true;
        }

        if(goDown)
        {
            transform.Translate(Vector3.down * Time.deltaTime * (downwardForce+speedRNG));
        }
        else
        {
            transform.Translate(Vector3.up * Time.deltaTime * (upwardForce+speedRNG));
        }

        if(goForward)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * (horizontalForce+speedRNG));
        }
        else
        {
            transform.Translate(Vector3.back * Time.deltaTime * (horizontalForce+speedRNG));
        }
    }
}
