using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ControllerV3 : MonoBehaviour
{
    [SerializeField] float speed;
    Vector3 newPos;
    Quaternion newRot;

    [Header("Scripts")]
    PlayerActions inputs;
    private Rigidbody playerBody;

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
        camRotation.x += ctx.ReadValue<Vector2>().x;
        camRotation.z += ctx.ReadValue<Vector2>().y;
    }

    #endregion

    void Movement()
    {
        newPos.x = transform.position.x + moveDir.x * speed;
        newPos.y = transform.position.y;
        newPos.z = transform.position.z + moveDir.y * speed;

        //playerBody.(newPos);
    }

    void Rotation()
    {
        //newRot = Quaternion.Euler(camRotation);

        transform.rotation = Quaternion.Euler(camRotation);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Movement();
        Rotation();

    }
}
