using System.Collections;
using NUnit.Framework.Api;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] float bufferTime;
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpSpeedMultiplier;

    [Header("Step")]
    [SerializeField] float bottomStepReach;
    [SerializeField] float topStepReach;
    [SerializeField] float stepRayHeight;
    [SerializeField] float maxStepHeight;
    [SerializeField] float stepSpeed;
    [SerializeField] float maxFallDepthClip;

    [Header("Collision")]
    [SerializeField] float wallPushforce;
    [SerializeField] float collisionHeight;
    [SerializeField] float collisionDistance;
    [SerializeField] float diagonalCollisionModifier;

    [Header("Camera")]
    [SerializeField] GameObject cameraPivot;
    [SerializeField] float sensibility;
    [SerializeField] Vector2 maxCamAngle;
    [SerializeField] Vector3 viewRotation;

    [Header("Physics")]
    [SerializeField] Vector3 launchSpeed;
    [SerializeField] float groundDrag;
    [SerializeField] float pushDivider;
    [SerializeField] float maxKnockbackForce;
    [SerializeField] float forceSpeedMultiplier;


    [Header("Private")]
    private LayerMask collisionMask;
    private Vector3 bottomPos;
    private Vector2 currInputDir;
    private bool touchStep;
    private bool isJumping;
    private bool canJump = true;
    private bool isGrounded;
    private bool isWalkingFwd;
    private bool isWalkingSide;
    private bool isBuffering;
    private Vector3 fallSpeed;
    private float jumpTime = 0.4f;
    private float baseJumpTime;
    private float pushSpeed;
    private float baseBufferTime;
    private float currMoveSpeed;

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
        if (isWalkingFwd)
        {
            currSpeed = (transform.forward + CheckCollision(currInputDir)) * currInputDir.x * currMoveSpeed;
            transform.Translate(currSpeed, Space.World);
        }

        if (isWalkingSide)
        {
            currSpeed = (transform.right + CheckCollision(currInputDir)) * currInputDir.y * currMoveSpeed;
            transform.Translate(currSpeed, Space.World);
        }
    }
    #endregion

    #region Jump
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (canJump)
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

        pushSpeed = jumpSpeedMultiplier;
    }

    void ApplyJump()
    {
        //When does the gravity takes the lead
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
        }
        else
        {
            isGrounded = false;
        }
    }

    Vector3 CheckCollisionFwd(Vector2 dir)
    {
        Vector3 collisionHeightVector = new Vector3(transform.position.x, transform.position.y - collisionHeight, transform.position.z);
        RaycastHit hit;

        switch (dir.x)
        {
            case 1:
                if (Physics.Raycast(collisionHeightVector, transform.rotation * Vector3.forward, out hit, collisionDistance, collisionMask))
                    return hit.normal;
                break;

            case -1:
                if (Physics.Raycast(collisionHeightVector, transform.rotation * -Vector3.forward, out hit, collisionDistance, collisionMask))
                    return hit.normal;
                break;
        }
        return Vector3.zero;
    }

    bool CheckCollisionSide(Vector2 dir)
    {
        Vector3 collisionHeightVector = new Vector3(transform.position.x, transform.position.y - collisionHeight, transform.position.z);
        float wallPush = currMoveSpeed * wallPushforce;

        switch (dir.y)
        {
            case 1:
                if (Physics.Raycast(collisionHeightVector, transform.rotation * Vector3.right, collisionDistance, collisionMask))
                {
                    transform.Translate(transform.rotation * Vector3.left * wallPush, Space.World);
                    return false;
                }
                break;

            case -1:
                if (Physics.Raycast(collisionHeightVector, transform.rotation * Vector3.left, collisionDistance, collisionMask))
                {
                    transform.Translate(transform.rotation * Vector3.right * wallPush, Space.World);
                    return false;
                }
                break;
        }
        return true;
    }

    Vector3 CheckCollision(Vector2 dir)
    {
        Vector3 collisionHeightVector = new Vector3(transform.position.x, transform.position.y - collisionHeight, transform.position.z);
        RaycastHit hit;

        switch (dir.x, dir.y)
        {
            case (1, 0):
                if (Physics.Raycast(collisionHeightVector, transform.rotation * Vector3.forward, out hit, collisionDistance, collisionMask))
                    return hit.normal;
                break;

            case (-1, 0):
                if (Physics.Raycast(collisionHeightVector, transform.rotation * Vector3.back, out hit, collisionDistance, collisionMask))
                    return -hit.normal;
                break;

            case (0, 1):
                if (Physics.Raycast(collisionHeightVector, transform.rotation * Vector3.right, out hit, collisionDistance, collisionMask))
                    return hit.normal;
                break;

            case (0, -1):
                if (Physics.Raycast(collisionHeightVector, transform.rotation * Vector3.left, out hit, collisionDistance, collisionMask))
                    return -hit.normal;
                break;

            case (1, 1):
                if (Physics.Raycast(collisionHeightVector, transform.rotation * (Vector3.forward + Vector3.right), out hit, collisionDistance, collisionMask))
                    return hit.normal;
                break;

            case (-1, 1):
                if (Physics.Raycast(collisionHeightVector, transform.rotation * (Vector3.back + Vector3.right), out hit, collisionDistance, collisionMask))
                    return new Vector3(-hit.normal.x, hit.normal.y, hit.normal.z);
                break;

            case (1, -1):
                if (Physics.Raycast(collisionHeightVector, transform.rotation * (Vector3.forward + Vector3.left), out hit, collisionDistance, collisionMask))
                    return hit.normal;
                break;

            case (-1, -1):
                if (Physics.Raycast(collisionHeightVector, transform.rotation * (Vector3.back + Vector3.left), out hit, collisionDistance, collisionMask))
                    return -hit.normal;
                break;
        }

        return Vector3.zero;
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
    public void Throw(Vector3 dir)
    {
        //The controller is launched into orbit, so I reduce the force
        Vector3 forceDir = dir.normalized;
        float forceMag = Mathf.Clamp(dir.magnitude, 0, maxKnockbackForce);
        launchSpeed = forceDir * (forceMag / pushDivider);

        fallSpeed.y = launchSpeed.y;
        launchSpeed.y = 0;

        pushSpeed = forceSpeedMultiplier;
    }

    void ApplyPhysics()
    {
        if (!isGrounded || isJumping)
        {
            fallSpeed.y += gravity * Time.deltaTime;
            canJump = false;
        }
        else if (isGrounded && fallSpeed.y <= 0)
        {
            canJump = true;

            fallSpeed.y = 0;
            launchSpeed = Vector3.MoveTowards(launchSpeed, Vector3.zero, groundDrag * Time.deltaTime);
        }
    
        transform.position += (launchSpeed + fallSpeed) * (pushSpeed * Time.deltaTime);
    }

    #endregion

    //Update
    void FixedUpdate()
    {
        CheckGround();

        if (currSpeed.x != 0 || currSpeed.z != 0)
        {
            CheckStep();

            if (touchStep)
                ApplyStep();
        }

        if (isBuffering)
            BufferTimer();

        if (isJumping)
            ApplyJump();

        ApplyPhysics();
        ApplyMovement();
    }
}
