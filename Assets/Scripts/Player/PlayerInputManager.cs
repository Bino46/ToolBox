using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Scripts")]
    PlayerActions inputs;
    ControllerV2 playerController;
    Headbutt playerHit;
    GrabObject grab;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GetComponent<ControllerV2>();
        playerHit = GetComponent<Headbutt>();
        grab = GetComponentInChildren<GrabObject>();

        inputs.Movement.Forward.performed += playerController.MovePlayerForward;
        inputs.Movement.Forward.canceled += playerController.MovePlayerForward;
        inputs.Movement.Right.performed += playerController.MovePlayerSide;
        inputs.Movement.Right.canceled += playerController.MovePlayerSide;

        inputs.Movement.View.performed += playerController.MoveCamera;

        inputs.Movement.Sprint.performed += playerController.Sprint;
        inputs.Movement.Sprint.canceled += playerController.Sprint;

        inputs.Movement.Jump.performed += playerController.Jump;

        inputs.Movement.Attack.performed += playerHit.ChargeHead;
        inputs.Movement.Attack.canceled += playerHit.SlingHead;

        inputs.Movement.Grab.performed += grab.Grab;
        inputs.Movement.Grab.canceled += grab.UnGrab;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
