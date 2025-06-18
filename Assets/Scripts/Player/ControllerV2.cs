using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class ControllerV2 : MonoBehaviour
{
    [Header("Movement")]
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
    [SerializeField] float sensibility;

    [Header("Private")]
    
    [SerializeField] Vector3 currSpeed;
    [SerializeField] Vector3 viewRotation;
    private CinemachineCamera fpvCamera;
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
    private Vector3 fallSpeed;
    private float jumpTime = 0.4f;
    private float baseJumpTime;
    private float baseBufferTime;
    private float currMoveSpeed;

    void Start()
    {
        fpvCamera = gameObject.transform.GetComponentInChildren<CinemachineCamera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        baseJumpTime = jumpTime;
        currMoveSpeed = walkSpeed;
    }

    //Input
    public void MovePlayerForward(InputAction.CallbackContext ctx)
    {
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

    public void MoveCamera(InputAction.CallbackContext ctx)
    {
        viewRotation.y += ctx.ReadValue<Vector2>().x * sensibility * Time.deltaTime;
        viewRotation.x += -ctx.ReadValue<Vector2>().y * sensibility * Time.deltaTime;

        fpvCamera.transform.eulerAngles = viewRotation;
        transform.eulerAngles = new Vector3(0, viewRotation.y, 0);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (canJump)
                Jump();
            else
            {
                baseBufferTime = bufferTime;
                isBuffering = true;
            }
        }
        
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            currMoveSpeed = sprintSpeed;
        else if (ctx.canceled)
            currMoveSpeed = walkSpeed;
    }
    //Values
    void ApplyMovement()
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

        if (isJumping)
        {
            baseJumpTime -= Time.deltaTime;

            if (baseJumpTime <= 0)
            {
                isJumping = false;
                baseJumpTime = jumpTime;
            }
        }

        if (isWalkingFwd)
            ForwardMovement();

        if (isWalkingSide)
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

    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.localPosition, Vector3.down, out hit, 1.1f))
        {
            isGrounded = true;

            if (hit.distance <= maxFallDepthClip)
                gameObject.transform.position += Vector3.up * Time.deltaTime;
        }
        else
            isGrounded = false;
    }

    void CheckCollision()
    {
        Vector3 bottomCollisionHeightVector = new Vector3(transform.position.x, transform.position.y - bottomCollisionHeight, transform.position.z);
        Vector3 topCollisionHeightVector = new Vector3(transform.position.x, transform.position.y - topCollisionHeight, transform.position.z);

        //Forward
        if (Physics.Raycast(bottomCollisionHeightVector, transform.rotation * Vector3.forward, bottomReach) || Physics.Raycast(topCollisionHeightVector, transform.rotation * Vector3.forward, topReach))
            currInputBlock[0] = true;
        else
            currInputBlock[0] = false;

        //Behind
        if (Physics.Raycast(bottomCollisionHeightVector, transform.rotation * -Vector3.forward, bottomReach) || Physics.Raycast(topCollisionHeightVector, transform.rotation * -Vector3.forward, topReach))
            currInputBlock[1] = true;
        else
            currInputBlock[1] = false;

        //Left
        if (Physics.Raycast(bottomCollisionHeightVector, transform.rotation * Vector3.left, bottomReach) || Physics.Raycast(topCollisionHeightVector, transform.rotation * Vector3.left, topReach))
            currInputBlock[2] = true;
        else
            currInputBlock[2] = false;

        //Right
        if (Physics.Raycast(bottomCollisionHeightVector, transform.rotation * -Vector3.left, bottomReach) || Physics.Raycast(topCollisionHeightVector, transform.rotation * -Vector3.left, topReach))
            currInputBlock[3] = true;
        else
            currInputBlock[3] = false;
    }

    void CheckStep()
    {
        bottomPos.x = gameObject.transform.position.x;
        bottomPos.y = gameObject.transform.position.y - stepRayHeight;
        bottomPos.z = gameObject.transform.position.z;

        if (Physics.Raycast(bottomPos, transform.rotation * Vector3.forward, bottomStepReach) || Physics.Raycast(bottomPos, transform.rotation * Vector3.left, bottomStepReach))
            touchStep = true; 

        if (Physics.Raycast(bottomPos, transform.rotation * -Vector3.forward, bottomStepReach) || Physics.Raycast(bottomPos, transform.rotation * -Vector3.left, bottomStepReach))
            touchStep = true; 
    }

    void ApplyStep()
    {
        touchStep = false;
        bool canStep = false;

        float startYpos = transform.position.y - stepRayHeight;
        float maxStep = transform.position.y + maxStepHeight;
        float currY = startYpos;

        Vector3 origin = new Vector3(transform.position.x, currY, transform.position.z);

        for (int i = 0; i < 100; i++)
        {
            origin.y = currY;

            bool hitFwd = Physics.Raycast(origin, transform.rotation * -Vector3.forward, topStepReach) || Physics.Raycast(origin, transform.rotation * Vector3.forward, topStepReach);
            bool hitLeft = Physics.Raycast(origin, transform.rotation * -Vector3.left, topStepReach) || Physics.Raycast(origin, transform.rotation * Vector3.left, topStepReach);

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

    void BufferTimer()
    {
        if (baseBufferTime > 0)
            baseBufferTime -= Time.deltaTime;
        else
            isBuffering = false;

        if (isBuffering && isGrounded)
            Jump();
    }

    void Jump()
    {
        fallSpeed.y = jumpHeight;
        isJumping = true;
        canJump = false;
    }

    void FixedUpdate()
    {
        CheckGround();
        CheckCollision();

        if (currSpeed.x != 0 || currSpeed.z != 0)
        {
            CheckStep();

            if (touchStep)
                ApplyStep();
        }

        if (isBuffering)
            BufferTimer();

        ApplyMovement();
    }
}
