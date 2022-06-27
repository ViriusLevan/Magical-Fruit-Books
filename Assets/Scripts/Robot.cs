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
    [SerializeField] protected Transform target;

    public enum EnemyType
    {
        Cylin, Rolly
    }
    protected Dictionary<EnemyType,int> scoreTable;
    public EnemyType et;

    [SerializeField] private GameObject enemyContainer;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        scoreTable = new Dictionary<EnemyType, int>(){
            {EnemyType.Cylin,8},{EnemyType.Rolly,5}
        };

        if(target==null){
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        GameObject container = Instantiate(enemyContainer, GameManager.Instance.enemyParent);
        transform.parent = container.transform;
        barTransform = container.transform.GetChild(0);
        healthBar = container.transform.GetChild(0).GetChild(0)
            .GetChild(0).GetChild(1).GetComponent<Image>();
    }
    Transform barTransform;
    // Update is called once per frame
    protected virtual void Update()
    {
        barTransform.position = transform.position;
        barTransform.LookAt(target);
    }

    public int GetHealth()
    {
        return health;
    }

    protected void Die(){
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        robotDeath?.Invoke(scoreTable[et]);
        Destroy(transform.parent.gameObject);
    }

    public void TakeDamage(int dAmount)
    {
        health-=dAmount;
        if(healthBar!=null)
            healthBar.fillAmount = 1.0f* health/maxHealth;
        if(health<=0)
            Die();
    }
}
