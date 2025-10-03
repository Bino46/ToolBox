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
    public enum ControllerType{Magic, RPG}
    [SerializeField] ControllerType controllerType;

    [Header("Magic controller")]
    [SerializeField] GameObject playerUi;

    [Header("Hidden variables")]
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
    
        inputs.Movement.Forward.performed += playerController.MovePlayerForward;
        inputs.Movement.Forward.canceled += playerController.MovePlayerForward;
        inputs.Movement.Right.performed += playerController.MovePlayerSide;
        inputs.Movement.Right.canceled += playerController.MovePlayerSide;

        inputs.Movement.View.performed += playerController.MoveCamera;


        inputs.Movement.Sprint.performed += playerController.Sprint;
        inputs.Movement.Sprint.canceled += playerController.Sprint;

        inputs.Movement.Jump.performed += playerController.Jump;

        SetupSpecifics();
    }

    void SetupSpecifics()
    {
        switch (controllerType)
        {
            case ControllerType.Magic:
                SetupMagic();
                break;

            case ControllerType.RPG:
                SetupRPG();
                break;
        }
    }

    void SetupMagic()
    {
        playerHit = GetComponent<Headbutt>();
        grab = GetComponentInChildren<GrabObject>();
        spell = GetComponent<ShootSpell>();
        SpellCraftUI spellInterface = playerUi.GetComponent<SpellCraftUI>();

        inputs.Movement.RightClick.performed += spellInterface.ResetHoldingSprite;
        inputs.Movement.MousePosition.performed += spellInterface.GetMousePos;

        inputs.Movement.ShowSpellMenu.performed += spellInterface.ShowMenu;

        inputs.Movement.ShowSpellMenu.performed += spell.UpdateAllProjectiles;

        inputs.Movement.SwitchWeapon.performed += SwitchAttack;

        // inputs.Movement.Grab.performed += grab.Grab;
        // inputs.Movement.Grab.canceled += grab.UnGrab; 
    }

    void SetupRPG()
    {

    }

    void SwitchAttack(InputAction.CallbackContext ctx)
    {
        if(!UIManager._instance.inMenu)
            switchMain = !switchMain;
        
        if (switchMain)
        {
            // inputs.Movement.Attack.performed -= playerHit.ChargeHead;
            // inputs.Movement.Attack.canceled -= playerHit.SlingHead;

            inputs.Movement.Attack.performed += spell.Shoot;

            UIManager._instance.ChangeTextOnUi("MainAttack", "Spell");
        }
        else
        {
            // inputs.Movement.Attack.performed += playerHit.ChargeHead;
            // inputs.Movement.Attack.canceled += playerHit.SlingHead;

            inputs.Movement.Attack.performed -= spell.Shoot;

            UIManager._instance.ChangeTextOnUi("MainAttack", " ");
        }
    }
}
