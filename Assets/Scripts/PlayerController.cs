using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    private float charHorSpeed=0.0f;
    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    public enum HoldingState {None,Apple,Banana,Watermelon};
    public HoldingState currentlyHolding = HoldingState.None;
    public Dictionary<Book.FruitType,int> nOfFruits;
    public TextMeshProUGUI bookNameText, appleNText,bananaNText,watermelonNText;
    private bool isBookOpen;

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

    private void UpdateNText()
    {
        appleNText.text =       nOfFruits[Book.FruitType.Apple].ToString();
        bananaNText.text =      nOfFruits[Book.FruitType.Banana].ToString();
        watermelonNText.text =  nOfFruits[Book.FruitType.Watermelon].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isBookOpen){
            UpdateMouseLook();
            UpdateMovement();
            ObjectRaycast();
            InteractInput();
            WeaponInput();
        }
    }

    void WeaponInput() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
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
            if(hit.transform.GetComponent<Book>()!=null)
            {
                bookNameText.gameObject.SetActive(true);
                Book temp = hit.transform.GetComponent<Book>();
                if(temp.fruitBookType==Book.FruitType.Apple)
                {
                    bookNameText.text="Apple";
                }
                else if(temp.fruitBookType==Book.FruitType.Banana)
                {
                    bookNameText.text="Banana";
                }
                else
                {
                    bookNameText.text="Watermelon";
                }
            }
            else
            {
                bookNameText.gameObject.SetActive(false);
            }
        }
        else
        {
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
}
