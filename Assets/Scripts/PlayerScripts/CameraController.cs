using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Tooltip("Enables to move the camera when holding the right mouse button. Doesn't work controller.")]
    public bool clickToMoveCamera = false;
    [Tooltip("Enables zoom in and out with the scroll wheel. Again, does not work with controller")]
    public bool canZoom = true;
    [Space]
    [Tooltip("Higher the sensitivity, faster the camera moves. if using a controller, increase value")]
    public float sensitivity = 0.7f;

    //Defines the maximum the camera can rotate in the Y axis, basically how far it can move up and down.
    [Tooltip("Camera Y rotation limits, X is the max it can go up and Y  is the max it can go down")]
    public Vector2 cameraLimit = new Vector2 (-45, 40);

    //Mouse coordinates on the screen
    float mouseX;
    float mouseY;

    //Minimum camera offset (zoom), so it can enter the character body
    float offsetDistanceY;

    //Get the player position, rotation, scale...
    Transform player;

    //Input actions variables
    PlayerInputActions actions;

    Vector2 lookInput;
    float zoomInput;
    bool rotateHoldInput;

    private void Awake()
    {
        actions = new PlayerInputActions();

        actions.Camera.Look.performed += OnLook;
        actions.Camera.Look.canceled += OnLookCanceled;

        actions.Camera.Zoom.performed += OnZoom;
        actions.Camera.Zoom.canceled += OnZoomCanceled;

        actions.Camera.RotateHold.performed += OnRotateHold;
        actions.Camera.RotateHold.canceled += OnRotateHold;
    }


    void Start()
    {
        //if moving the camera with click is disabled, hide the mouse, as the player will not use it
        if(!clickToMoveCamera)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked; //locks the mouse to the center of the screen
            UnityEngine.Cursor.visible = false;
        }

        //Get the player object when starting
        player = GameObject.FindWithTag("Player").transform;

        //Set the min distance between the camera and the character
        offsetDistanceY = transform.position.y;
    }

    void Update()
    {
        //Make the camera position same as the player but with the offset I defined
        //the camera will follow the character but keeping the distance
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);
        
        //if camera zoom is enabled, set the zoom when mouse wheel is scrolled
        if(canZoom && zoomInput != 0)
        {
            // To zoom in and out we change the field of view, its easier than changing the camera transform
            // Scroll up = zoom in (FOV decreases), scroll down = zoom out (FOV increases)
            Camera.main.fieldOfView -= zoomInput * sensitivity * 2;
        }

        //if the move camera with mouse is enabled
        if(clickToMoveCamera)
        {
            //Check if right click is being pressed, if not, the update ends
            if(!rotateHoldInput)
            {
                return;
            }
        }

        //Get and calculate the mouse position on screen
        //horizontal rotation
        mouseX += lookInput.x * sensitivity;
        //vertical rotation
        mouseY += lookInput.y * sensitivity;

        //This limits how far the camera can rotate on the character, if i dont use this the camera can do a 360 on the character
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        //And this makes the calculated position of the cursor rotate the camera
        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }

    void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }

    void OnZoomCanceled(InputAction.CallbackContext context)
    {
        zoomInput = 0f;
    }

    void OnRotateHold(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            rotateHoldInput = true;
        }
        if(context.canceled)
        {
            rotateHoldInput = false;
        }
    }

    void OnEnable()
    {
        actions.Camera.Enable();
    }

    void OnDisable()
    {
        actions.Camera.Disable();
    }


}
