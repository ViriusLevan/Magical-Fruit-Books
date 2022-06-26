using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField]private Transform[] spawnPoints;
    [SerializeField]private GameObject[] enemyPrefabs;
    [SerializeField]private float spawnCooldown=20f;
    [SerializeField]private float spawnCDMinimum=4f;
    private float spawnCountdown = 0f;
    private int score=0;
    [SerializeField]private TextMeshProUGUI scoreText;

    [SerializeField]private float bookCooldown=5f;
    private float bookCountdown=0f;
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private GameObject goPanel;
    private bool playerIsDead=false;
    
    [SerializeField] private TextMeshProUGUI faceText, appleNText,bananaNText,watermelonNText;
    private int enemyCount=2;
    private int bookCount=5;
    [SerializeField] private int enemyLimit=50;
    [SerializeField] private int bookLimit =100;
    [SerializeField] private Transform enemyParent, bookParent;

    // Start is called before the first frame update
    void Start()
    {
        Robot.robotDeath+=IncreaseScore;
        PlayerController.playerDies+=PlayerDeath;
        PlayerController.fruitNChanged+=UpdateNText;
        PlayerController.changeFaceText+=ChangeFaceText;
        PlayerController.hideFaceText+=HideFaceText;
        PlayerController.showFaceText+=ShowFaceText;
        Book.bookDestroyed+=ReduceBookCount;
    }

    void OnDestroy(){
        Robot.robotDeath-=IncreaseScore;
        PlayerController.playerDies-=PlayerDeath;
        PlayerController.fruitNChanged-=UpdateNText;
        PlayerController.changeFaceText-=ChangeFaceText;
        PlayerController.hideFaceText-=HideFaceText;
        PlayerController.showFaceText-=ShowFaceText;
        Book.bookDestroyed-=ReduceBookCount;
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerIsDead){
            SpawnBook();
            SpawnEnemies();
        }
    }
    
    public void PlayerDeath(){
        playerIsDead=true;
        goPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void IncreaseScore(int increment)
    {
        enemyCount-=1;
        score+=increment;
        scoreText.text=score.ToString();
    }

    private bool doubleSpawned=false;
    private void SpawnEnemies(){
        if(spawnCountdown>0f)
        {
            spawnCountdown-=Time.deltaTime;
        }
        else
        {
            if(enemyCount<enemyLimit){
                spawnCountdown=spawnCooldown;
                //chance to immediately spawn another enemy
                if(!doubleSpawned && Random.Range(0,10)>6)
                {
                    spawnCountdown=0;
                    doubleSpawned=true;
                }
                else if(doubleSpawned)
                {
                    doubleSpawned=false;    
                }
                if(spawnCooldown>spawnCDMinimum+0.5f)
                    spawnCooldown-=0.5f;
                int rngPoint = UnityEngine.Random.Range(0,2);
                int rngType = UnityEngine.Random.Range(0,2);
                GameObject inst = Instantiate(enemyPrefabs[rngType], 
                    spawnPoints[rngPoint].position, Quaternion.identity, enemyParent);
                inst.name = inst.name.Replace("(Clone)","").Trim();
                enemyCount+=1;
            }
        }
    }

    private void ReduceBookCount()
    {
        bookCount-=1;
    }

//Spawn Area
    //y30 z75 x40->x-40
    //y30 z-75 x40->x-40
    private bool has2xBookTrig=false;
    private void SpawnBook(){
        if(bookCountdown>0f)
        {
            bookCountdown-=Time.deltaTime;
        }else
        {
            if(bookCount<bookLimit){
                //chance to immediately spawn another book
                if(!has2xBookTrig && Random.Range(0,10)>6)
                {
                    spawnCountdown=spawnCooldown;
                    has2xBookTrig=true;
                }
                else if(has2xBookTrig)
                {
                    has2xBookTrig=false;
                }
                bookCountdown=bookCooldown;
                int rngLR = UnityEngine.Random.Range(0,2);
                int rngX = UnityEngine.Random.Range(-40,41);
                int rngType = UnityEngine.Random.Range(0,3);
                Vector3 newPos = new Vector3(rngX, 40, rngLR>0?75:-75);
                GameObject inst = Instantiate(bookPrefab, newPos, Quaternion.identity, bookParent);
                inst.GetComponent<Book>().SwitchBookType(rngType);
                bookCount+=1;
            }
        }
    }

    public void RestartButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void UpdateNText(Dictionary<Book.FruitType,int> nOfFruits)
    {
        appleNText.text =       nOfFruits[Book.FruitType.Apple].ToString();
        bananaNText.text =      nOfFruits[Book.FruitType.Banana].ToString();
        watermelonNText.text =  nOfFruits[Book.FruitType.Watermelon].ToString();
    }

    private void ChangeFaceText(string name)
    {
        faceText.gameObject.SetActive(true);
        faceText.text = name;
    }

    private void ShowFaceText()
    {
        if(!faceText.gameObject.activeSelf)
        {
            faceText.gameObject.SetActive(true);
        }
    }

    private void HideFaceText()
    {
        if(faceText.gameObject.activeSelf)
        {
            faceText.gameObject.SetActive(false);
        }
    }

}
