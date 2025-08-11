using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Scripts")]
    PlayerActions inputs;
    ControllerV2 playerController;
    Headbutt playerHit;
    GrabObject grab;
    ShootSpell spell;
    [SerializeField] SpellCraftUI playerUi;

    [Header("Variables")]
    bool switchMain;

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
        spell = GetComponent<ShootSpell>();

        inputs.Movement.Forward.performed += playerController.MovePlayerForward;
        inputs.Movement.Forward.canceled += playerController.MovePlayerForward;
        inputs.Movement.Right.performed += playerController.MovePlayerSide;
        inputs.Movement.Right.canceled += playerController.MovePlayerSide;

        inputs.Movement.View.performed += playerController.MoveCamera;

        inputs.Movement.MousePosition.performed += playerUi.GetMousePos;
        inputs.Movement.ShowSpellMenu.performed += playerUi.ShowMenu;

        inputs.Movement.Sprint.performed += playerController.Sprint;
        inputs.Movement.Sprint.canceled += playerController.Sprint;

        inputs.Movement.Jump.performed += playerController.Jump;

        inputs.Movement.SwitchWeapon.performed += SwitchAttack;

        inputs.Movement.Grab.performed += grab.Grab;
        inputs.Movement.Grab.canceled += grab.UnGrab; 
    }

    void SwitchAttack(InputAction.CallbackContext ctx)
    {
        if(!UIManager._instance.inMenu)
            switchMain = !switchMain;
        
        if (switchMain)
        {
            inputs.Movement.Attack.performed -= playerHit.ChargeHead;
            inputs.Movement.Attack.canceled -= playerHit.SlingHead;

            inputs.Movement.Attack.performed += spell.Shoot;

            UIManager._instance.ChangeTextOnUi("MainAttack", "Spell");
        }
        else
        {
            inputs.Movement.Attack.performed += playerHit.ChargeHead;
            inputs.Movement.Attack.canceled += playerHit.SlingHead;

            inputs.Movement.Attack.performed -= spell.Shoot;

            UIManager._instance.ChangeTextOnUi("MainAttack", "Headbutt");
        }
    }
}
