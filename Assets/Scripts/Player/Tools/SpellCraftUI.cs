using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[Serializable]
public class SelectionSlot
{
    public GameObject selectedSlot;
    //public AddedBehavior behavior;
    public bool isFull = false;
    Image image;
    string spellHoldingName;

    public Image GetImage()
    {
        Transform child = selectedSlot.transform.GetChild(0);
        image = child.GetComponent<Image>();
        return image;
    }

    public void SetName(string spellAdded, int id)
    {
        spellHoldingName = spellAdded + id.ToString();
    }

    public string GetName()
    {
        return spellHoldingName;
    }
}

public class SpellCraftUI : MonoBehaviour
{
    [SerializeField] ControllerV2 player;
    [SerializeField] LoadSpellFromRune runeSlots;
    public static SpellCraftUI _instance;

    [Header("UI objects")]
    [SerializeField] Image spellSelected;
    [SerializeField] GameObject spellMenu;
    [SerializeField] Image menuButton;
    [SerializeField] TextMeshProUGUI currModifiyingSpellName;
    [SerializeField] GameObject[] menuParent = new GameObject[2];
    [SerializeField] GameObject[] menuLists = new GameObject[4];
    [SerializeField] Sprite[] buttonSelection = new Sprite[2];
    public SelectionSlot[] behaviorsButtons = new SelectionSlot[3];
    public SelectionSlot projectileButton;
    [SerializeField] GameObject modSlotParent;
    Button[] modSlotsUI = new Button[16]; 

    [Header("Script values")]
    // Reference to sprites in addon menu
    [SerializeField] Sprite nullSprite;
    int currSelectedSlot; //reference
    Vector2 mousePos;
    int currHoldingSpellId = -1;
    int holdingSpellCount;
    bool swicthMenu;
    bool inSpellMenu;
    bool selectProjectile;
    bool selectBehavior;
    bool selectModifier;
    bool modProj;

    void Awake()
    {
        _instance = this;
    }
    
    private void Start()
    {
        for (int i = 0; i < modSlotsUI.Length; i++)
        {
            modSlotsUI[i] = modSlotParent.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }

    void Update()
    {
        if (inSpellMenu && (selectProjectile || selectBehavior || selectModifier))
        {
            Vector3 mouseWorld = new Vector3(mousePos.x, mousePos.y, 0.05f);
            spellSelected.transform.position = Camera.main.ScreenToWorldPoint(mouseWorld);
        }
    }

    #region Inputs
    public void ShowMenu(InputAction.CallbackContext ctx)
    {
        ResetHoldingSprite();

        spellMenu.SetActive(!spellMenu.activeSelf);
        inSpellMenu = spellMenu.activeSelf;
        UIManager._instance.inMenu = inSpellMenu;
    }
    public void GetMousePos(InputAction.CallbackContext ctx)
    {
        if (inSpellMenu)
        {
            mousePos = ctx.ReadValue<Vector2>();
        }
    }

    #endregion

    #region Behavior
    public void SelectBehavior(int id)
    {
        selectBehavior = true;

        spellSelected.color = Color.white;
        spellSelected.sprite = runeSlots.so_behaviors[id].tex;
        spellSelected.preserveAspect = true;

        currHoldingSpellId = id;
    }

    public void SlotBehavior(int id)
    {
        if (!selectBehavior)
        {
            if (behaviorsButtons[id].isFull && holdingSpellCount >= 4)
            {
            //!Magic number for slot amount-------------------------^
                currModifiyingSpellName.text = behaviorsButtons[id].GetName();
                currSelectedSlot = id;

                ResetModInterface();
                LoadModifiersOnUI(id);
                SwitchToBHModifiers();
            }
        }
        else
            DepositBehavior(id);
    }

    void DepositBehavior(int id)
    {
        if (selectBehavior)
        {
            behaviorsButtons[id].GetImage().sprite = spellSelected.sprite;

            runeSlots.LoadBehavior(currHoldingSpellId, id);
            behaviorsButtons[id].SetName(runeSlots.GetName(), id);

            if (!behaviorsButtons[id].isFull)
                holdingSpellCount++;

            behaviorsButtons[id].isFull = true;
            behaviorsButtons[id].selectedSlot.GetComponentInChildren<SpellGlowMask>().ActivateSpell();

            //behaviorsButtons[id].behavior = runeSlots.so_behaviors[id];

            ResetHoldingSprite();
        }
    }

    #endregion

    #region Projectile

    public void SelectProjectile(int id)
    {
        selectProjectile = true;

        spellSelected.color = Color.white;
        spellSelected.sprite = runeSlots.so_projectiles[id].tex;
        spellSelected.preserveAspect = true;

        currHoldingSpellId = id;
    }


    public void SlotProjectile()
    {
        if (!selectProjectile)
        {
            if (projectileButton.isFull && holdingSpellCount == 4)
            {
                currModifiyingSpellName.text = projectileButton.GetName();

                ResetModInterface();
                LoadModifiersOnUI(0);
                SwitchToPJModifiers();
            }
        }
        else
            DepositProjectile();
    }

    void DepositProjectile()
    {
        if (selectProjectile)
        {
            projectileButton.GetImage().sprite = spellSelected.sprite;

            runeSlots.LoadProjectile(currHoldingSpellId);
            projectileButton.SetName(runeSlots.GetName(),0);

            if(!projectileButton.isFull)
                holdingSpellCount++;

            projectileButton.isFull = true;
            projectileButton.selectedSlot.GetComponentInChildren<SpellGlowMask>().ActivateSpell();

            ResetHoldingSprite();
        }
    }
    #endregion

    #region Modifiers
    public void SelectModifier(int id)
    {
        selectModifier = true;

        spellSelected.color = Color.white;
        spellSelected.sprite = runeSlots.so_modifiers[id].tex;
        spellSelected.preserveAspect = true;

        currHoldingSpellId = id;
    }

    public void DepositModifier(int id)
    {
        if (selectModifier)
        {
            modSlotsUI[id - 1].GetComponentInChildren<Image>().sprite = spellSelected.sprite;

            modSlotsUI[id]?.gameObject.SetActive(true);

            if (modProj)
                runeSlots.LoadProjectileModifier(id, currHoldingSpellId);
            else
                runeSlots.LoadBehaviorModifiers(currSelectedSlot, id, currHoldingSpellId);

            ResetHoldingSprite();
        }
    }

    void LoadModifiersOnUI(int id)
    {
        List<AddedBehavior> modList = runeSlots.GetModifiersOnBehavior(id);

        int lastId = 0;
        Transform modSlot;

        for (int i = 0; i < modList.Count; i++)
        {
            modSlot = modSlotParent.transform.GetChild(i);
            modSlot.gameObject.SetActive(true);
            modSlot.GetComponentInChildren<Image>().sprite = modList[i].tex;
            lastId = i;
        }

        modSlot = modSlotParent.transform.GetChild(lastId + 1);

        if (modSlot != null && modList.Count > 0)
        {
            modSlot.gameObject.SetActive(true);
            modSlot.GetComponentInChildren<Image>().sprite = nullSprite;
        }
    }
    #endregion

    #region UI menus

    public void SwitchActionMenu()
    {
        ResetHoldingSprite();

        if (swicthMenu)
        {
            swicthMenu = false;

            menuLists[0].SetActive(true);
            menuLists[1].SetActive(false);

            menuButton.sprite = buttonSelection[0];
        }
        else
        {
            swicthMenu = true;

            menuLists[0].SetActive(false);
            menuLists[1].SetActive(true);

            menuButton.sprite = buttonSelection[1];
        }
    }

    void SwitchToBHModifiers()
    {
        ResetHoldingSprite();

        modProj = false;

        menuParent[0].SetActive(false);
        menuParent[1].SetActive(true);

        menuLists[2].SetActive(true);
        menuLists[3].SetActive(false);
    }

    void SwitchToPJModifiers()
    {
        ResetHoldingSprite();

        modProj = true;

        menuParent[0].SetActive(false);
        menuParent[1].SetActive(true);

        menuLists[2].SetActive(false);
        menuLists[3].SetActive(true);
    }

    public void SwitchToBase()
    {
        ResetHoldingSprite();

        ResetModInterface();

        menuParent[0].SetActive(true);
        menuParent[1].SetActive(false);
    }

    public void ResetHoldingSprite(InputAction.CallbackContext ctx)
    {
        if (inSpellMenu && (selectProjectile || selectBehavior || selectModifier))
        {
            spellSelected.preserveAspect = false;
            Color trans = new Color(1, 1, 1, 0);
            spellSelected.color = trans;

            selectProjectile = false;
            selectBehavior = false;
            selectModifier = false;

            currHoldingSpellId = -1;
        }
    }
    public void ResetHoldingSprite()
    {
        if (inSpellMenu && (selectProjectile || selectBehavior || selectModifier))
        {
            spellSelected.preserveAspect = false;
            Color trans = new Color(1, 1, 1, 0);
            spellSelected.color = trans;

            selectProjectile = false;
            selectBehavior = false;
            selectModifier = false;

            currHoldingSpellId = -1;
        }
    }

    void ResetModInterface()
    {
        modSlotsUI[0].GetComponentInChildren<Image>().sprite = nullSprite;

        for (int i = 1; i < modSlotsUI.Length; i++)
        {
            modSlotsUI[i].GetComponentInChildren<Image>().sprite = nullSprite;
            modSlotsUI[i].gameObject.SetActive(false);
        }
    }

    #endregion

    public void FullColorSpell()
    {
        if (holdingSpellCount > behaviorsButtons.Length)
            projectileButton.selectedSlot.GetComponentInChildren<SpellGlowMask>().maxSize = 4;
        else
            projectileButton.selectedSlot.GetComponentInChildren<SpellGlowMask>().maxSize = 1.6f;
    }
}
