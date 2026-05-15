using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    //Public value variables

    [Tooltip("Speed at which the character moves.")]
    public float velocity = 5f;
    [Tooltip("Value added to velocity when the character is sprinting")]
    public float sprintAddition = 3.5f;
    [Tooltip("The higher this value, the higher the character will jump")]
    public float jumpForce = 18f;
    [Tooltip("Time the character stays in the air, the higher this value, the longer the character floats before falling")]
    public float jumpTime = 0.85f;


    [Space]
    [Tooltip("Force that moves the character down. Changing this value will affect all movement, such as jumping and falling.")]
    public float gravity = 9.8f;

    //Checks the character current state
    bool isJumping = false;
    bool isSprinting = false;
    bool isCrouching = false;

    //key input for the character
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputCrouch;
    bool inputSprint;

    //Inputactions, added later
    PlayerInputActions actions;
    Vector2 moveInput;
    bool jumpInput;
    bool sprintInput;
    bool crouchInput;


    //Character animations
    // Animator animator;
    //Gets the character collision and movement controller component
    CharacterController cc;

    //Controls the time the character spends in the air
    float jumpElapsedTime = 0;



    private void Awake()
    {
        actions = new PlayerInputActions();

        actions.Player.Move.performed += OnMove;
        actions.Player.Move.canceled += OnMoveCanceled;

        actions.Player.Jump.performed += OnJump;
        actions.Player.Jump.canceled += OnJump;

        actions.Player.Sprint.performed += OnSprint;
        actions.Player.Sprint.canceled += OnSprint;

        actions.Player.Crouch.performed += OnCrouch;
        actions.Player.Crouch.canceled += OnCrouch;

    }
    void Start()
    {
        //Gets the component of the animators and character controller
        cc = GetComponent<CharacterController>();
        //  animator = GetComponent<Animator>();
    }

    //This is going to only be used to identify inputs and trigger animations
    void Update()
    {
        //Input mapping
        inputHorizontal = moveInput.x;
        inputVertical = moveInput.y;
        inputJump = jumpInput;
        inputSprint = sprintInput;
        inputCrouch = crouchInput;

        //Check if i press the crouch input and change the players state
        if (inputCrouch)
        {
            isCrouching = !isCrouching; //its a toggle
            crouchInput = false; //resets, so its just 1 toggle per input, and doesnt flick between crouch, normal
        }

        if (cc.isGrounded)
        {
            //If its in the crouched state, runs the animation, if otherwise, not
            //Crouch does not shrink the character collider btw
            if (isCrouching == true)
            {
                //      animator.SetBool("crouch", true);
            }
            else
            {
                //    animator.SetBool("crouch", false);
            }

            //Check the player speed and if its high enough, it will trigger the run animation
            float minimumSpeed = 0.9f; //This is flexible, pending to test with other values
            if (cc.velocity.magnitude > minimumSpeed)
            {
                //      animator.SetBool("run", true);
            }
            else
            {
                //       animator.SetBool("run", false);
            }

            //Same as the run, but now with the sprint conditional
            if (cc.velocity.magnitude > minimumSpeed && inputSprint)
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }

            //After we do all the checks, we already know if the character is sprinting or not
            //       animator.SetBool("sprint", isSprinting);

        }

        //Jumping/airborne animation if not in the ground
        if (cc.isGrounded == true)
        {
            //      animator.SetBool("air", true);
        }
        else
        {
            //       animator.SetBool("air", false);
        }

        //Checks if the jump input is pressed and if the character is on the ground
        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
            //you can crouch while jumping, revisit this if relevant in the future
        }
        HeadHittingDetect();

    }

    //I use fixedupdate so the force and movement applied to the character is consistant, fps doesnt matter
    private void FixedUpdate()
    {
        //Checks if the character is sprinting, if it is, it adds the velocity
        float velocityAddition = 0;
        if (isSprinting)
        {
            velocityAddition = sprintAddition;
        }

        //Checks if the character is crouching, if it is apply a speed nerf
        if (isCrouching)
        {
            velocityAddition = -(velocity * 0.50f); //half speed nerf
        }

        //Movement amount for this frame on X and Z
        //(velocity + velocityAddition) = final speed (walk/sprint/crouch)
        //Time.deltaTime = converts speed-per-second into movement-per-frame
        float directionX = inputHorizontal * (velocity + velocityAddition) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAddition) * Time.deltaTime;
        //Y is upwards movement, so it stays at 0 or it would fly
        float directionY = 0;

        //Check if the player jumped
        if (isJumping)
        {
            //I use smoothStep to make the jump feel more natural
            // If i just do this: directionY = jumpForce * Time.deltaTime; it would be very awkward
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;

            //Increases and counts the time that has passed since the player started the jump
            jumpElapsedTime += Time.deltaTime;
            //And if the elapsed time surpasses the expected jump time, it makes the character fall
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }

        //After we calculate the jump and the movement, now we need to calculate and apply gravity
        //Needs to be negative so the game is always applying down force to the character
        directionY = directionY - gravity * Time.deltaTime;

        //MOVEMENT DONE, NOW ABOUT THE PLAYER ROTATING AND WORKING WELL WITH THE CAMERA


        //First, we're gonna locate where's the front and the right side of the character
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        //Obviously we will not rotate the character in the y axis xD
        forward.y = 0;
        right.y = 0;

        //Normalization makes the calculation uniform, if we dont do this the player would go faster diagonally
        //since the forces are going to add, per example, in minecraft happens this exact thing.
        forward.Normalize();
        right.Normalize();

        //Lets assignt the forward with the Z direction (towards the 3d depth) and the right with X, for lateral movement
        forward = forward * directionZ;
        right = right * directionX; //WE ONLY USE RIGHT BECAUSE LEFT ITS JUST RIGHT -1

        //if i dont add this the character is going to always be facing towards the center of the world when i stop pressing a key
        if (directionX != 0 || directionZ != 0)
        {
            //Calculate the direction the player wants to move in this case (forward + right)
            //Atan2 just converts that direction into an angle we can rotate toward, unity needs an angle to interpret which rotation we wanna give to the character
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            //Create a rotation that faces to that angle(in this case we only rotate around Y, because its NOT  relative to the camera, so we want it to "spin" to the sides
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            //Rotate the character towards the target direction, we use this to interpolate and making it smooth, if we dont have this it would just snap
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        //This is it with character rotation

        //This part just builds the final movement vector

        //In this case we combine jumping and gravity for the final vertical vector
        Vector3 verticalDirection = Vector3.up * directionY;
        //Here we combine horizontal movement for the final horizontal vector
        Vector3 horizontalDirection = forward + right;
        //Creates the full 3D movement vector
        Vector3 movement = verticalDirection + horizontalDirection;
        //We apply this vector to the character controller so it actually moves.
        cc.Move(movement);
    }

    //this makes the character stop moving if he hits his head with something
    //If i dont add this, and i jump towards a ceiling, you can see the character floating for a bit before it starts falling down
    void HeadHittingDetect()
    {
        //Distance above the head to check
        float headHitDistance = 1.1f;
        //Center of the character
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        //how far up i check, in this case is half the height of the character and a little bit extended, just in case
        float hitCalc = cc.height / 2f * headHitDistance;

        // Debug.DrawRay(ccCenter, Vector3.up * headHeight, Color.red);
        //If the raycast hits something, that means theres a ceiling above
        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }



    //CALLBACKS FOR THE INPUT ACTIONS
    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }
    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput = true;
        }
        if (context.canceled)
        {
            jumpInput = false;
        }
    }
    void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            sprintInput = true;
        }
        if (context.canceled)
        {
            sprintInput = false;
        }
    }

    void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            crouchInput = true;
        }
    }


    void OnEnable()
    {
        actions.Player.Enable();
    }

    void OnDisable()
    {
        actions.Player.Disable();
    }

}
