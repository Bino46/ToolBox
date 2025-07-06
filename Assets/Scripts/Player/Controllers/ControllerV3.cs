
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerV3 : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float sensibility;
    [SerializeField] Vector2 maxCamAngle;
    Vector3 newPos;
    [SerializeField] float counterGravity;
    LayerMask collisionMask;

    [Header("Scripts")]
    PlayerActions inputs;
    Rigidbody playerBody;

    [Header("Input values")]
    private Vector2 moveDir;
    private Vector3 camRotation;

    void Awake()
    {
        inputs = new PlayerActions();
        playerBody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        collisionMask = LayerMask.GetMask("Walls");

        inputs.Movement.Forward.performed += GetInputForward;
        inputs.Movement.Right.performed += GetInputSide;
        inputs.Movement.Forward.canceled += GetInputForward;
        inputs.Movement.Right.canceled += GetInputSide;

        inputs.Movement.View.performed += GetCameraView;
    }

    #region Input Values
    void GetInputForward(InputAction.CallbackContext ctx)
    {
        moveDir.x = ctx.ReadValue<float>();
    }

    void GetInputSide(InputAction.CallbackContext ctx)
    {
        moveDir.y = ctx.ReadValue<float>();
    }

    void GetCameraView(InputAction.CallbackContext ctx)
    {
        camRotation.y += ctx.ReadValue<Vector2>().x * sensibility;
        camRotation.x -= ctx.ReadValue<Vector2>().y * sensibility;
    }
    #endregion

    #region Movement

    void Movement()
    {
        // newPos.x = transform.position.x + moveDir.x * speed;
        // newPos.y = transform.position.y;
        // newPos.z = transform.position.z + moveDir.y * speed;

    }

    void Rotation()
    {
        camRotation.x = Mathf.Clamp(camRotation.x, maxCamAngle.x, maxCamAngle.y);
        camRotation.z = 0;
        transform.rotation = Quaternion.Euler(camRotation);
    }

    void CheckGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, collisionMask))
        {
            transform.position += Vector3.up * counterGravity;
        }
    }

    #endregion

    // Update is called once per frame
    private void FixedUpdate()
    {
        Rotation();
        Movement();

    }
}
