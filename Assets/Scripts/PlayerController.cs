using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] bool lockCursor = true;
    [SerializeField] float jumpSpeed = 12.0f;
    [SerializeField] float gravity = -13f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    [SerializeField] private float fireCooldown=0.5f;
    private float fireCounter=0f;
    [SerializeField] private float boostTime=5f;
    private float boostCountdown=0f;

    [SerializeField] private int playerHealth=100,maxPlayerHealth=100;

    float cameraPitch = 0.0f;
    private float velocityY = 0.0f, velocityX=0.0f;
    private float charHorSpeed=12.0f;
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
    private bool isBookOpen;
    public delegate void OnPlayerDeath();
    public static event OnPlayerDeath playerDies;
    public delegate void OnFruitNChanged(Dictionary<Book.FruitType,int> nFruits);
    public static event OnFruitNChanged fruitNChanged;
    public delegate void OnDifferentHitBook(string bookname);
    public static event OnDifferentHitBook changeFaceText;
    public delegate void OnTextNotNeeded();
    public static event OnTextNotNeeded hideFaceText,showFaceText;
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
    }

    void OnDestroy()
    {
        Typer.wordCompleted-=IncreaseFruit;
        Typer.bookClosed-=SetBookBoolFalse;
    }

    private void SetBookBoolFalse()
    {
        isBookOpen=false;
    }

    private void IncreaseFruit(Book.FruitType ft, int i)
    {
        nOfFruits[ft]+=i;
        fruitNChanged?.Invoke(nOfFruits);
        PlayFruitGainedSound((int) ft);
    }

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI playerHealthN;
    [SerializeField] private Image healthBar;

    public void TakeDamage(int damage)
    {
        playerHealth-=damage;
        if(playerHealth<=0)
        {
            playerDies?.Invoke();
            PlayDamageTakenSound(3);
        }
        else if(damage<=5)
        {
            PlayDamageTakenSound(2);
        }
        else
        {
            PlayDamageTakenSound(1);
        }
    }
    private void UpdateHealthBar()
    {
        playerHealthN.text=playerHealth.ToString();
        healthBar.fillAmount = 1.0f * playerHealth/maxPlayerHealth;
    }


    // Update is called once per frame
    void Update()
    {
        if(!isBookOpen){
            UpdateMouseLook();
            UpdateMovement();
            ObjectRaycast();
            WeaponInput();
        }
        UpdateHealthBar();
        UpdateBoostBar();
    }

    
    private void WeaponInput() {
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
        else if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            if(currentlyHolding==Book.FruitType.Apple && nOfFruits[currentlyHolding]>0)
            {
                AppleBoost();
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
        fruitNChanged?.Invoke(nOfFruits);
    }

    [Header("Boost")]
    [SerializeField] private Image boostBar;
    [SerializeField] private TextMeshProUGUI boostNText;
    [SerializeField] private GameObject boostPanel;
    private void AppleBoost()
    {
        nOfFruits[Book.FruitType.Apple]-=1;
        boostCountdown=boostTime;
        fruitNChanged?.Invoke(nOfFruits);
        PlayBoostSound();
    }
    private void UpdateBoostBar()
    {
        if(boostCountdown>0){
            boostPanel.SetActive(true);
            boostBar.fillAmount = boostCountdown/boostTime;
            boostNText.text= boostCountdown.ToString("0.00");
        }
        else
        {
            boostPanel.SetActive(false);
        }
    }

    [Header("Zero Distance SFX")]
    [SerializeField] private AudioClip[] fruitGains;
    [SerializeField] private AudioClip[] damageTaken;//or death
    [SerializeField] private AudioClip boostActivate;
    [SerializeField] private AudioSource fGainSource, damagedSource, boostSource;
    private void PlayFruitGainedSound(int i)
    {
        fGainSource.clip = fruitGains[i];
        fGainSource.Play();
    }
    private void PlayDamageTakenSound(int i)
    {
        damagedSource.clip = damageTaken[i];
        damagedSource.Play();
    }
    private void PlayBoostSound()
    {
        boostSource.Play();
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


    private String lastBookName="", lastRobotName="";
    private void ObjectRaycast()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 10f))
        {
            if(hit.transform.TryGetComponent(out Book temp))
            {
                if(!lastBookName.Equals(temp.name))
                {
                    lastBookName = temp.name;
                    changeFaceText?.Invoke($"{lastBookName}\nPress E to interact");
                }
                else
                {
                    showFaceText();
                }
                if(Input.GetKeyDown(KeyCode.E) && !isBookOpen && !temp.used)
                {
                    temp.OpenBook();
                    isBookOpen=true;
                    hideFaceText?.Invoke();
                }
            }
            else
            {
                hideFaceText?.Invoke();
            }
        }
        else if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 50f))
        {
            if(hit.transform.TryGetComponent(out Robot r))
            {
                if(!lastRobotName.Equals($"{hit.transform.gameObject.name} \nHP:{r.GetHealth()}"))
                {
                    lastRobotName = $"{hit.transform.gameObject.name} \nHP:{r.GetHealth()}";
                    changeFaceText?.Invoke(lastRobotName);
                }
                else
                {
                    showFaceText();
                }
            }
            else
            {
                hideFaceText?.Invoke();
            }
        }
        else{
            hideFaceText?.Invoke();
        }
    }

    
    private void UpdateMouseLook()
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

    
    private void UpdateMovement()
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
        if(boostCountdown>0)
        {
            velocityX = charHorSpeed*2f;
            boostCountdown-=Time.deltaTime;
        }
        else
        {
            velocityX = charHorSpeed;
        }

        Vector3 velocity =
            (transform.forward * currentDir.y + transform.right * currentDir.x) * velocityX
            + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }
}
