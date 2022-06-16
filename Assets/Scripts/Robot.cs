using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Robot : MonoBehaviour, IDamageable
{
    public float movespeed, turnRate, accelForce;
    public int health;
    public Rigidbody selfRB;
    public delegate void OnRobotDeath(int score);
    public static event OnRobotDeath robotDeath;
    public enum EnemyType
    {
        Cylin, Rolly
    }
    private Dictionary<EnemyType,int> scoreTable;
    public EnemyType et;

    // Start is called before the first frame update
    void Start()
    {
        scoreTable = new Dictionary<EnemyType, int>(){
            {EnemyType.Cylin,8},{EnemyType.Rolly,5}
        };
    }

    // Update is called once per frame
    void Update()
    {
    }


    protected void Die(){
        //TODO add explosion effect
        robotDeath?.Invoke(scoreTable[et]);
        Destroy(this.gameObject);
    }

    public void TakeDamage(int dAmount)
    {
        health-=dAmount;
        if(health<=0)
            Die();
    }
}
