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
    [SerializeField]private float spawnCDMinimum=5f;
    private float spawnCountdown = 0f;
    private int score=0;
    [SerializeField]private TextMeshProUGUI scoreText;

    [SerializeField]private float bookCooldown=7f;
    private float bookCountdown=0f;
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private GameObject goPanel;
    private bool playerIsDead=false;
    
    [SerializeField] private TextMeshProUGUI faceText, appleNText,bananaNText,watermelonNText;

    // Start is called before the first frame update
    void Start()
    {
        Robot.robotDeath+=IncreaseScore;
        PlayerController.playerDies+=PlayerDeath;
        PlayerController.fruitNChanged+=UpdateNText;
        PlayerController.changeFaceText+=ChangeFaceText;
        PlayerController.hideFaceText+=HideFaceText;
        PlayerController.showFaceText+=ShowFaceText;
    }

    void OnDestroy(){
        Robot.robotDeath-=IncreaseScore;
        PlayerController.playerDies-=PlayerDeath;
        PlayerController.fruitNChanged-=UpdateNText;
        PlayerController.changeFaceText-=ChangeFaceText;
        PlayerController.hideFaceText-=HideFaceText;
        PlayerController.showFaceText-=ShowFaceText;
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
        score+=increment;
        scoreText.text=score.ToString();
    }

    private void SpawnEnemies(){
        if(spawnCountdown>0f)
        {
            spawnCountdown-=Time.deltaTime;
        }
        else
        {
            spawnCountdown=spawnCooldown;
            if(spawnCooldown>spawnCDMinimum+0.5f)
                spawnCooldown-=0.5f;
            int rngPoint = UnityEngine.Random.Range(0,2);
            int rngType = UnityEngine.Random.Range(0,2);
            GameObject inst = Instantiate(enemyPrefabs[rngType], spawnPoints[rngPoint].position, Quaternion.identity);
            inst.name = inst.name.Replace("(Clone)","").Trim();
        }
    }

    //y30 z75 x40->x-40
    //y30 z-75 x40->x-40
    private void SpawnBook(){
        if(bookCountdown>0f)
        {
            bookCountdown-=Time.deltaTime;
        }else
        {
            bookCountdown=bookCooldown;
            int rngLR = UnityEngine.Random.Range(0,2);
            int rngX = UnityEngine.Random.Range(-40,41);
            int rngType = UnityEngine.Random.Range(0,3);
            Vector3 newPos = new Vector3(rngX, 40, rngLR>0?75:-75);
            GameObject inst = Instantiate(bookPrefab, newPos, Quaternion.identity);
            inst.GetComponent<Book>().SwitchBookType(rngType);
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
