using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ControllerV2 : MonoBehaviour
{
    Animator animPlayer;
    [SerializeField] Rigidbody body;
    [Header("Movement")]
    [SerializeField] Vector3 currSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float gravity;
    [SerializeField] float jumpHeight;
    [SerializeField] float bufferTime;

    [Header("Step")]
    [SerializeField] float bottomStepReach;
    [SerializeField] float topStepReach;
    [SerializeField] float stepRayHeight;
    [SerializeField] float maxStepHeight;
    [SerializeField] float stepSpeed;
    [SerializeField] float maxFallDepthClip;

    [Header("Collision")]
    [SerializeField] float bottomCollisionHeight;
    [SerializeField] float bottomReach;
    [SerializeField] float topCollisionHeight;
    [SerializeField] float topReach;

    [Header("Camera")]
    [SerializeField] GameObject cameraPivot;
    [SerializeField] float sensibility;
    [SerializeField] Vector2 maxCamAngle;
    [SerializeField] Vector3 viewRotation;

    [Header("Physics")]
    [SerializeField] Vector3 launchSpeed;
    [SerializeField] float airDrag;
    [SerializeField] float groundDrag;
    [SerializeField] float pushDivider;
    [SerializeField] float thresholdResetPhysics;

    [Header("Private")]
    private LayerMask collisionMask;
    private Vector3 bottomPos;
    private Vector2 currInputDir;
    private bool[] currInputBlock = new bool[4];
    private bool touchStep;
    private bool isJumping;
    private bool canJump = true;
    private bool isGrounded;
    private bool isWalkingFwd;
    private bool isWalkingSide;
    private bool isBuffering;
    private bool isBeingThrown;
    private Vector3 fallSpeed;
    private float jumpTime = 0.4f;
    private float baseJumpTime;
    private float baseBufferTime;
    private float currMoveSpeed;
    private float currDrag;
    private float currLaunchDelay;

    void Start()
    {
        //Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        baseJumpTime = jumpTime;
        currMoveSpeed = walkSpeed;

        collisionMask = LayerMask.GetMask("Walls");

        animPlayer = GetComponentInChildren<Animator>();
    }

    #region Movement
    public void MovePlayerForward(InputAction.CallbackContext ctx)
    {
        //Check the forward input value : 1 is forward, -1 backward and 0 static
        isWalkingFwd = true;

        switch (ctx.ReadValue<float>())
        {
            case 1:
                currInputDir.x = 1;
                break;
            case -1:
                currInputDir.x = -1;
                break;
            case 0:
                isWalkingFwd = false;

                currInputDir.x = 0;
                break;
        }
    }
    public void MovePlayerSide(InputAction.CallbackContext ctx)
    {
        //Check the side input value : 1 is right, -1 left and 0 static
        isWalkingSide = true;
        
        switch (ctx.ReadValue<float>())
        {
            case 1:
                currInputDir.y = 1;
                break;
            case -1:
                currInputDir.y = -1;
                break;
            case 0:
                isWalkingSide = false;

                currInputDir.y = 0;
                break;
        }
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        float sprinting = ctx.ReadValue<float>();

        if (sprinting == 1)
            currMoveSpeed = sprintSpeed;
        else
            currMoveSpeed = walkSpeed;
    }

    void ApplyMovement()
    {
        //Separated both for readibility
        if (isWalkingFwd && !isBeingThrown)
            ForwardMovement();

        if (isWalkingSide && !isBeingThrown)
            SideMovement();
    }

    void ForwardMovement()
    {
        if (currInputDir.x == 1 && currInputBlock[0] == false)
        {
            currSpeed = transform.forward;
            transform.Translate(currSpeed * currMoveSpeed, Space.World);
        }
        else if (currInputDir.x == -1 && currInputBlock[1] == false)
        {
            currSpeed = -transform.forward;
            transform.Translate(currSpeed * currMoveSpeed, Space.World);
        }
    }

    void SideMovement()
    {
        if (currInputDir.y == 1 && currInputBlock[3] == false)
        {
            currSpeed = transform.right;
            transform.Translate(currSpeed * currMoveSpeed, Space.World);
        }
        else if (currInputDir.y == -1 && currInputBlock[2] == false)
        {
            currSpeed = -transform.right;
            transform.Translate(currSpeed * currMoveSpeed, Space.World);
        }
    }
    #endregion

    #region Jump
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (canJump && !isBeingThrown)
            JumpAction();
        else
        {
            baseBufferTime = bufferTime;
            isBuffering = true;
        }  
    }

        void BufferTimer()
    {
        if (baseBufferTime > 0)
            baseBufferTime -= Time.deltaTime;
        else
            isBuffering = false;

        if (isBuffering && isGrounded)
            JumpAction();
    }

    void JumpAction()
    {
        fallSpeed.y = jumpHeight;
        isJumping = true;
        canJump = false;
    }

    void ApplyJump()
    {
        if (isJumping)
        {
            baseJumpTime -= Time.deltaTime;

            if (baseJumpTime <= 0)
            {
                isJumping = false;
                baseJumpTime = jumpTime;
            }
        }
    }

    #endregion

    #region Camera
    public void MoveCamera(InputAction.CallbackContext ctx)
    {
        viewRotation.y += ctx.ReadValue<Vector2>().x * sensibility * Time.deltaTime;
        viewRotation.x += -ctx.ReadValue<Vector2>().y * sensibility * Time.deltaTime;

        viewRotation.x = Mathf.Clamp(viewRotation.x, maxCamAngle.x, maxCamAngle.y);

        cameraPivot.transform.eulerAngles = viewRotation;

        transform.eulerAngles = new Vector3(0, viewRotation.y, 0);
    }
    #endregion

    #region Collison
    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.localPosition, Vector3.down, out hit, 1.1f, collisionMask))
        {
            isGrounded = true;

            if (hit.distance <= maxFallDepthClip)
                gameObject.transform.position += Vector3.up * Time.deltaTime;

            currDrag = groundDrag;

            isBeingThrown = false;
        }
        else
        {
            isGrounded = false;
            currDrag = airDrag;
        }
    }

    void CheckCollision()
    {
        //Checking a layer "Wall" in 4 directions both at the top and at the feet of the controller
        //TODO compensate direction rather than block the input

        Vector3 bottomCollisionHeightVector = new Vector3(transform.position.x, transform.position.y - bottomCollisionHeight, transform.position.z);
        Vector3 topCollisionHeightVector = new Vector3(transform.position.x, transform.position.y - topCollisionHeight, transform.position.z);

        //Forward
        if (Physics.Raycast(bottomCollisionHeightVector, transform.rotation * Vector3.forward, bottomReach, collisionMask) || Physics.Raycast(topCollisionHeightVector, transform.rotation * Vector3.forward, topReach, collisionMask))
            currInputBlock[0] = true;
        else
            currInputBlock[0] = false;

        //Behind
        if (Physics.Raycast(bottomCollisionHeightVector, transform.rotation * -Vector3.forward, bottomReach, collisionMask) || Physics.Raycast(topCollisionHeightVector, transform.rotation * -Vector3.forward, topReach, collisionMask))
            currInputBlock[1] = true;
        else
            currInputBlock[1] = false;

        //Left
        if (Physics.Raycast(bottomCollisionHeightVector, transform.rotation * Vector3.left, bottomReach, collisionMask) || Physics.Raycast(topCollisionHeightVector, transform.rotation * Vector3.left, topReach, collisionMask))
            currInputBlock[2] = true;
        else
            currInputBlock[2] = false;

        //Right
        if (Physics.Raycast(bottomCollisionHeightVector, transform.rotation * -Vector3.left, bottomReach, collisionMask) || Physics.Raycast(topCollisionHeightVector, transform.rotation * -Vector3.left, topReach, collisionMask))
            currInputBlock[3] = true;
        else
            currInputBlock[3] = false;
    }
    #endregion

    #region Step
    void CheckStep()
    {
        //Check at the bottom of the controller in 4 directions if there is a heigth difference
        //TODO perhaps add 4 diagonal raycasts for smoothing 

        bottomPos.x = gameObject.transform.position.x;
        bottomPos.y = gameObject.transform.position.y - stepRayHeight;
        bottomPos.z = gameObject.transform.position.z;

        if (Physics.Raycast(bottomPos, transform.rotation * Vector3.forward, bottomStepReach, collisionMask) || Physics.Raycast(bottomPos, transform.rotation * Vector3.left, bottomStepReach,collisionMask))
            touchStep = true; 

        if (Physics.Raycast(bottomPos, transform.rotation * -Vector3.forward, bottomStepReach,collisionMask) || Physics.Raycast(bottomPos, transform.rotation * -Vector3.left, bottomStepReach,collisionMask))
            touchStep = true; 
    }

    void ApplyStep()
    {
        //Before checking another step, I use another raycast that goes up by a little every frame.
        //If that raycast stops hitting a wall/ground within a threshold, it calculates the heigth difference and lifts the controller by that value
        touchStep = false;
        bool canStep = false;

        float startYpos = transform.position.y - stepRayHeight;
        float maxStep = transform.position.y + maxStepHeight;
        float currY = startYpos;

        Vector3 origin = new Vector3(transform.position.x, currY, transform.position.z);

        //For loop to avoid crashes
        for (int i = 0; i < 100; i++)
        {
            origin.y = currY;

            bool hitFwd = Physics.Raycast(origin, transform.rotation * -Vector3.forward, topStepReach, collisionMask) || Physics.Raycast(origin, transform.rotation * Vector3.forward, topStepReach, collisionMask);
            bool hitLeft = Physics.Raycast(origin, transform.rotation * -Vector3.left, topStepReach, collisionMask) || Physics.Raycast(origin, transform.rotation * Vector3.left, topStepReach, collisionMask);

            if (!hitFwd && !hitLeft)
            {
                canStep = true;
                break;
            }

            currY += stepSpeed * Time.deltaTime;

            if (currY > maxStep)
                break;
        }

        if (canStep && (isWalkingFwd || isWalkingSide))
        {
            float climbAmount = currY - startYpos;
            transform.position += new Vector3(0, climbAmount, 0);
        }
    }
    #endregion

#region Physics
    //Physics
    void CheckForce()
    {
        //Check if children rigidbody "feels" a force
        if ((body.GetAccumulatedForce().x > 1 || body.GetAccumulatedForce().y > 1 || body.GetAccumulatedForce().z > 1) && !isBeingThrown)
        {
            isGrounded = false;
            Throw(body.GetAccumulatedForce());
        }
    }

    void Throw(Vector3 dir)
    {
        //The controller is launched into orbit, so I reduce the force
        launchSpeed = dir / pushDivider;
        isBeingThrown = true;
    }

    void ApplyPhysics()
    {
        //Reducing every value by a bit until it stops
        if (launchSpeed.x < thresholdResetPhysics && launchSpeed.x > -thresholdResetPhysics)
            launchSpeed.x = 0;
        else
            launchSpeed.x -= currDrag * Mathf.Sign(launchSpeed.x) * Time.deltaTime;

        if (launchSpeed.y > thresholdResetPhysics)
            launchSpeed.y += gravity * Time.deltaTime;
        else
            launchSpeed.y = 0;

        if (launchSpeed.z < thresholdResetPhysics && launchSpeed.z > -thresholdResetPhysics)
            launchSpeed.z = 0;
        else
            launchSpeed.z -= currDrag * Mathf.Sign(launchSpeed.z) * Time.deltaTime;

        transform.position += launchSpeed;
    }

    void Gravity()
    {
        if (!isGrounded || isJumping)
        {
            fallSpeed.y += gravity * Time.deltaTime;
            transform.Translate(fallSpeed, Space.World);
        }
        else
        {
            canJump = true;
            fallSpeed.y = 0;
        }
    }

    #endregion

    //Update
    void FixedUpdate()
    {   
        CheckGround();
        CheckCollision();
        CheckForce();

        if (currSpeed.x != 0 || currSpeed.z != 0)
        {
            CheckStep();

            if (touchStep)
                ApplyStep();
        }

        if (isBuffering)
            BufferTimer();

        Gravity();

        if (isJumping)
            ApplyJump();

        ApplyPhysics();
        ApplyMovement();
    }
}
