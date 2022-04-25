using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : MonoBehaviour
{

 
    #region Screen Properties (For Mobiles)
    [Header("Screen Properties (For Mobiles)")]
    public bool fullScreen;
    [Range(0,100)] [Tooltip("Use it for define the range in case of dont generate a fullscreen JoyStick")]public int amount;
    [Tooltip("Invert the position of the JoyStick left to right in case of using split screen")] public bool Invert;
    [Tooltip("This is if an 3D camera")]
    public Camera cameraCanvas; 
    #endregion
    [Space]
    [Header("Other Properties")]
    [Tooltip("JoyStick sprites")] public GameObject stick; 
    [Tooltip("JoyStick sprites")] public GameObject panel;
    [Tooltip("Object or Character for use the JoyStick")]public GameObject characterToMove;
    [Tooltip("Test the speed of the object")][SerializeField]private float moveSpeed;
    private Rigidbody rb;
    private float screenWidth;
    private float screenHeight;

    private int leftTouch = 99;
    private Vector2 touchPosition;
    private Vector2 moveDirection;
    private float quantityToAdd;
    private void Awake()
    {
        
        screenHeight = Camera.main.orthographicSize * 2f;
        screenWidth = screenHeight * ((float)Screen.width / (float)Screen.height);
    }
    void Start()
    {
        //JoyStick Sprite
        stick.SetActive(false);
        panel.SetActive(false);

        //Character or Object
        /* Change it for character controller*/

        rb = characterToMove.GetComponent<Rigidbody>();
  
    }

    // Update is called once per frame
    void Update()
    {
        testMode();

        if(Time.timeScale != 0)
        {
            if (!fullScreen)
            {
              
                ControllerPadDividedScreen();
            }
            else
            {
                ControllerPadFullScreen();
               
            } 
        }
    }



    private void ControllerPadDividedScreen()
    {
        int i = 0;
       
      
        while (i < Input.touchCount)
        {
            Touch t = Input.GetTouch(i);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(t.position);
            
                float percentOfScreen = (float)amount / 100f;
                float rangeOfScreen = (screenWidth * percentOfScreen) + (screenWidth / -2); 

                if(t.phase== TouchPhase.Began)
                {
                 
                    if ((touchPos.x >= rangeOfScreen &&!Invert)|| (touchPos.x< rangeOfScreen && Invert))
                    {
                    //Do something else of move 
                    characterToMove.GetComponent<SpriteRenderer>().sharedMaterial.color = Random.ColorHSV(); 
                    }
                    else
                    {
                        leftTouch = t.fingerId;
                        touchPosition = touchPos;
                        panel.SetActive(true);
                        stick.SetActive(true);
                        panel.transform.position = touchPosition;
                        stick.transform.position = touchPosition;
                    }
                    
                }
                else if(t.phase == TouchPhase.Moved && leftTouch == t.fingerId)
                {
                    stick.transform.position = touchPos;
                    stick.transform.position = new Vector2(
                        Mathf.Clamp(stick.transform.position.x,
                        panel.transform.position.x - 0.5f,
                        panel.transform.position.x + 0.5f),
                        Mathf.Clamp(stick.transform.position.y,
                        panel.transform.position.y - 0.5f,
                        panel.transform.position.y + 0.5f)
                        );
                    moveDirection = (stick.transform.position - panel.transform.position).normalized;
                    rb.velocity = moveDirection * moveSpeed;
                }
                else if((t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)&& leftTouch == t.fingerId)
                {
                    leftTouch = 99;
                    panel.SetActive(false);
                    stick.SetActive(false);
                    rb.velocity = Vector2.zero; 
                }
                i++; 
        }
    }

    private void ControllerPadFullScreen()
    {
        int i = 0;
        while (i < Input.touchCount)
        {
            Touch t = Input.GetTouch(i);
            Vector2 touchPos = cameraCanvas.ScreenToWorldPoint(t.position); //Camera.main.ScreenToWorldPoint(t.position);
            Debug.Log("La posicion es: " + touchPos); 

            float percentOfScreen = amount / 100;
            float rangeOfScreen = (screenWidth * percentOfScreen) + (screenWidth / -2);
            if (t.phase == TouchPhase.Began)
            {
              leftTouch = t.fingerId;
              touchPosition = touchPos;
              panel.SetActive(true);
              stick.SetActive(true);
              panel.transform.position = touchPosition;
              stick.transform.position = touchPosition;
            }
            else if (t.phase == TouchPhase.Moved && leftTouch == t.fingerId)
            {
                stick.transform.position = touchPos;
                stick.transform.position = new Vector2(
                    Mathf.Clamp(stick.transform.position.x,
                    panel.transform.position.x - 0.5f,
                    panel.transform.position.x + 0.5f),
                    Mathf.Clamp(stick.transform.position.y,
                    panel.transform.position.y - 0.5f,
                    panel.transform.position.y + 0.5f)
                    );
                moveDirection = (stick.transform.position - panel.transform.position).normalized;
                rb.velocity = new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed;
            }
            else if ((t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) && leftTouch == t.fingerId)
            {
                leftTouch = 99;
                panel.SetActive(false);
                stick.SetActive(false);
                rb.velocity = Vector2.zero;
            }
            i++;
        }
    }

    private void testMode()
    {


        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector3(0, 0f, 1f)*moveSpeed;
        }
        

        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector3(1f,0,0) * moveSpeed;

        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector3(-1f, 0, 0) * moveSpeed;

        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector3(0, 0, -1f) * moveSpeed;
        }
       
    }

}
