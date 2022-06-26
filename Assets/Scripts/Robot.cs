using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Robot : MonoBehaviour, IDamageable
{
    [SerializeField]protected float movespeed,turnRate,accelForce;
    [SerializeField]protected int health, maxHealth;
    
    [SerializeField]protected Rigidbody selfRB;
    public delegate void OnRobotDeath(int score);
    public static event OnRobotDeath robotDeath;
    [SerializeField] protected GameObject explosionPrefab;
    [SerializeField] protected Image healthBar;

    public enum EnemyType
    {
        Cylin, Rolly
    }
    protected Dictionary<EnemyType,int> scoreTable;
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

    public int GetHealth()
    {
        return health;
    }


    protected void Die(){
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        robotDeath?.Invoke(scoreTable[et]);
        Destroy(this.gameObject);
    }

    public void TakeDamage(int dAmount)
    {
        health-=dAmount;
        healthBar.fillAmount = 1.0f* health/maxHealth;
        if(health<=0)
            Die();
    }
}
