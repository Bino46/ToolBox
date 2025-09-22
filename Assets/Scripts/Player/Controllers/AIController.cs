using System.Threading;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.InputSystem;

public class AIController : MonoBehaviour
{
    PlayerActions inputs;
    float lastDirectionPressed;
    [SerializeField] float speed;
    [SerializeField] float gravity;
    [SerializeField] Vector3 movementDir;
    public enum PlayerState { Grounded, Jump, Fall, Coyote, Dash }
    public PlayerState currState;

    [Header("Jump")]
    [SerializeField] float jumpMinStrength;
    [SerializeField] float jumpHoldStrength;
    [SerializeField] float coyoteTime;
    [SerializeField] float maxJumpTime;
    float coyote;
    float jumpTime;
    bool canDash;

    [Header("Dash")]
    [SerializeField] float dashTime;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashCooldown;
    float currDashTime;
    float currCooldown;
    bool touchedGround;

    [Header("Collisions")]
    Bounds bounds;
    LayerMask collisionMask;
    float skinWidth = 0.015f;

    #region System

    void Awake()
    {
        inputs = new PlayerActions();
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    void Start()
    {
        inputs.Movement.Right.performed += Move;
        inputs.Movement.Right.canceled += Move;

        inputs.Movement.Jump.performed += Jump;
        inputs.Movement.Jump.canceled += Jump;

        inputs.Movement.Sprint.performed += Dash;
        inputs.Movement.Sprint.canceled += Dash;

        bounds = GetComponent<CapsuleCollider>().bounds;
        bounds.Expand(-2 * skinWidth);
        collisionMask = LayerMask.GetMask("Walls");
    }

    void FixedUpdate()
    {
        if (currState == PlayerState.Dash)
            DashOver();
        else
        {
            MoveOver();

            if(currState != PlayerState.Jump)
                CheckGround();
        }

        if (currState == PlayerState.Jump)
            JumpOver();
        else if (currState != PlayerState.Grounded && currState != PlayerState.Dash)
            Gravity();

        if (!canDash)
        {
            currCooldown -= Time.deltaTime;

            if (currCooldown <= 0 && touchedGround)
                canDash = true;
        }
    }

    void SwitchState(PlayerState newState)
    {
        currState = newState;

        Debug.Log("Switch to " + newState.ToString());

        switch (newState)
        {
            case PlayerState.Coyote:
                coyote = coyoteTime;
                break;
        }
    }

    #endregion

    #region Action
    public void Move(InputAction.CallbackContext ctx)
    {
        if (currState != PlayerState.Dash)
        {
            lastDirectionPressed = ctx.ReadValue<float>();
            movementDir.x = lastDirectionPressed * speed;
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && currState == PlayerState.Grounded || currState == PlayerState.Coyote)
        {
            movementDir.y = 0;
            movementDir.y += jumpMinStrength;
            jumpTime = maxJumpTime;

            SwitchState(PlayerState.Jump);
        } 
        else if(currState == PlayerState.Jump)
            SwitchState(PlayerState.Fall);
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (currState != PlayerState.Dash && canDash)
        {
            SwitchState(PlayerState.Dash);
            canDash = false;
            touchedGround = false;

            movementDir.x = lastDirectionPressed * dashSpeed;

            currDashTime = dashTime;
            movementDir.y = 0;
        }
    }

    #endregion

    #region Physics

    void MoveOver()
    {
        transform.position += new Vector3(CollideAndSlide(movementDir, transform.position), movementDir.y, movementDir.z);
    }

    void JumpOver()
    {
        movementDir.y += jumpHoldStrength;

        jumpTime -= Time.deltaTime;

        if (jumpTime < 0)
            SwitchState(PlayerState.Fall);

    }

    void DashOver()
    {
        currDashTime -= Time.deltaTime;

        if (currDashTime < 0)
        {
            movementDir.x = lastDirectionPressed * speed;
            currCooldown = dashCooldown;
            SwitchState(PlayerState.Coyote);
        }

        transform.position += new Vector3(CollideAndSlide(movementDir, transform.position), movementDir.y, movementDir.z);
    }

    float CollideAndSlide(Vector3 vel, Vector3 pos)
    {
        float dist = vel.magnitude + skinWidth;

        //collision on the sides, might be overkill
        RaycastHit hit;
        if (Physics.SphereCast(pos, bounds.extents.x, vel.normalized, out hit, dist, collisionMask))
        {
            Vector3 snapToSurface = vel.normalized * (hit.distance - skinWidth);

            if (snapToSurface.magnitude <= skinWidth)
                snapToSurface = Vector3.zero;

            return snapToSurface.x;
        }

        return vel.x;
    }

    void Gravity()
    {
        movementDir.y -= gravity;
    }

    void CheckGround()
    {
        RaycastHit hit;

        //check ground
        if (Physics.Raycast(transform.position, Vector3.down, out hit, bounds.size.y, collisionMask))
        {
            movementDir.y = 0;
            SwitchState(PlayerState.Grounded);
            coyote = coyoteTime;

            touchedGround = true;

            CheckIfInGround(hit);
        }
        else if (currState != PlayerState.Jump && currState != PlayerState.Fall)
        {
            //give little coyote time
            coyote -= Time.deltaTime;
            SwitchState(PlayerState.Coyote);
        }

        //check roof
        if (Physics.Raycast(transform.position, Vector3.up, out hit, bounds.size.y, collisionMask))
        {
            movementDir.y = -0.1f;
        }

        //stop coyote time
        if (coyote <= 0 && movementDir.y < 0)
            SwitchState(PlayerState.Fall);
    }

    void CheckIfInGround(RaycastHit hitInfo)
    {
        if (hitInfo.distance < bounds.size.y * 0.85f)
            transform.position += Vector3.up * Time.deltaTime * 3;
    }
    #endregion
}
