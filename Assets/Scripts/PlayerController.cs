using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] bool lockCursor = true;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] float jumpSpeed = 8.0f;
    [SerializeField] float runSpeed = 12.0f;
    [SerializeField] float gravity = -13f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    [SerializeField] private float fireCooldown=0.5f;
    private float fireCounter=0f;

    [SerializeField] private int playerHealth=100;
    private bool isDead=false;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    private float charHorSpeed=0.0f;
    [SerializeField]CharacterController controller;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    public Transform holdingPoint;
    public Book.FruitType currentlyHolding = Book.FruitType.Apple;
    public GameObject[] fruitsHeld;
    public GameObject[] fruits;
    public Dictionary<Book.FruitType,int> nOfFruits;
    public TextMeshProUGUI bookNameText, appleNText,bananaNText,watermelonNText;
    private bool isBookOpen;

    private float sWheelInput=0f;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        nOfFruits = new Dictionary<Book.FruitType, int>()
        {
          {Book.FruitType.Apple,0}
          ,{Book.FruitType.Banana,0}
          ,{Book.FruitType.Watermelon,0}
        };
        Typer.wordCompleted+=IncreaseFruit;
        Typer.bookClosed+=SetBookBoolFalse;
        SwitchFruitModel((int)currentlyHolding);
        //TODO move to manager
        Robot.robotDeath+=IncreaseScore;
    }

    void OnDestroy()
    {
        Typer.wordCompleted-=IncreaseFruit;
        Typer.bookClosed-=SetBookBoolFalse;
        //TODO move to manager
        Robot.robotDeath-=IncreaseScore;
    }

    private void SetBookBoolFalse()
    {
        isBookOpen=false;
    }

    private void IncreaseFruit(Book.FruitType ft, int i)
    {
        nOfFruits[ft]+=i;
        UpdateNText();
    }

    public TextMeshProUGUI playerHealthN;

    public void TakeDamage(int damage)
    {
        playerHealth-=damage;
        playerHealthN.text=playerHealth.ToString();
        if(playerHealth<=0)
        {
            Die();
        }
    }
    private void Die(){
        isDead=true;
        goPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateNText()
    {
        appleNText.text =       nOfFruits[Book.FruitType.Apple].ToString();
        bananaNText.text =      nOfFruits[Book.FruitType.Banana].ToString();
        watermelonNText.text =  nOfFruits[Book.FruitType.Watermelon].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead){
            if(!isBookOpen){
                UpdateMouseLook();
                UpdateMovement();
                ObjectRaycast();
                InteractInput();
                WeaponInput();
            }
            //TODO move these to manager
            SpawnBook();
            SpawnEnemies();
        }
    }

    void WeaponInput() {
        sWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKeyDown(KeyCode.Q)
            || sWheelInput>0)
        {
            HoldingStateAdvance(true);
        }
        else if(sWheelInput<0)
        {
            HoldingStateAdvance(false);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(nOfFruits[currentlyHolding]>0 && fireCounter<=0)
            {
                FireFruit();
                fireCounter=fireCooldown;
            }
        }
        if(fireCounter>0){fireCounter-=Time.deltaTime;}
    }

    private void FireFruit()
    {
        int ind = (int)currentlyHolding;
        Vector3 newPos = playerCamera.transform.position + (playerCamera.transform.forward*1f);
        Instantiate(fruits[ind], newPos, playerCamera.transform.rotation);
        nOfFruits[currentlyHolding]-=1;
        UpdateNText();
    }

    private void HoldingStateAdvance(bool next)
    {
        if(next)
        {
            if((int)currentlyHolding < Enum.GetValues(typeof(Book.FruitType)).Length-1)
            {
                int nextEnumI = (int)currentlyHolding+1;
                currentlyHolding = ((Book.FruitType)nextEnumI);
            }else{
                currentlyHolding = Book.FruitType.Apple;  
            }
        }
        else if(!next)
        {
            if((int)currentlyHolding>0)
            {
                int nextEnumI = (int)currentlyHolding-1;
                currentlyHolding = ((Book.FruitType)nextEnumI);
            }else{
                currentlyHolding = Book.FruitType.Watermelon;  
            }
        }
        SwitchFruitModel((int)currentlyHolding);
    }

    private void SwitchFruitModel(int index)
    {
        for (int i = 0; i < fruitsHeld.Length; i++)
        {
            if(i!=index)
            {
                fruitsHeld[i].SetActive(false);
            }
            else
            {
                fruitsHeld[i].SetActive(true);
            }
        }
    }

    void InteractInput()
    {
        if(isBookOpen)
        {
            return;
        }
        RaycastHit hit;
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 5f))
            {
                if(hit.transform.GetComponent<Book>()!=null)
                {
                    bookNameText.gameObject.SetActive(true);
                    Book temp = hit.transform.GetComponent<Book>();
                    temp.OpenBook();
                    isBookOpen=true;
                    bookNameText.gameObject.SetActive(false);
                }
            }
        }
    }

    void ObjectRaycast()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 5f))
        {
            if(hit.transform.TryGetComponent(out Book temp))
            {
                bookNameText.gameObject.SetActive(true);
                if(temp.fruitBookType==Book.FruitType.Apple)
                {
                    bookNameText.text="Apple \nPress E to Open";
                }
                else if(temp.fruitBookType==Book.FruitType.Banana)
                {
                    bookNameText.text="Banana \nPress E to Open";
                }
                else
                {
                    bookNameText.text="Watermelon \nPress E to Open";
                }
            }
            else
            {
                bookNameText.gameObject.SetActive(false);
            }
        }
        else if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 50f))
        {
            if(hit.transform.TryGetComponent(out Robot r))
            {
                bookNameText.gameObject.SetActive(true);
                Robot temp = hit.transform.GetComponent<Robot>();
                bookNameText.text=$"{hit.transform.gameObject.name} \nHP:{temp.health}";
            }
            else
            {
                bookNameText.gameObject.SetActive(false);
            }
        }
        else{
            bookNameText.gameObject.SetActive(false);
        }
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta =
            Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta
            , ref currentMouseDeltaVelocity, mouseSmoothTime);
        
        //float before = transform.rotation.x;

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80.0f, 80.0f);

        // playerCamera.localEulerAngles = new Vector3(
        //     1 * cameraPitch, transform.localEulerAngles.y, 0);
            
        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);

        //float after = transform.rotation.x;
        //Debug.Log($"{before}->{after} when X={currentMouseDelta.x} and Y={cameraPitch}");
    }

    void UpdateMovement()
    {
        Vector2 targetDir =
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir =
            Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded)
        {
            velocityY = 0.0f;
            if(Input.GetButton("Jump")){
                velocityY = jumpSpeed;
            }
        }
        velocityY += gravity * Time.deltaTime;

        if(Input.GetButton("Fire3")){
            charHorSpeed=runSpeed;
        }else{
            charHorSpeed=walkSpeed;
        }

        Vector3 velocity =
            (transform.forward * currentDir.y + transform.right * currentDir.x) * charHorSpeed
            + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }

    //I'm tired
    //TODO move these to gamemanager after game jam finishes
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
            Instantiate(enemyPrefabs[rngType], spawnPoints[rngPoint].position, Quaternion.identity);
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
}
