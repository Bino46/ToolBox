using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[Serializable]
public class SelectionSlot
{
    public GameObject selectedSlot;
    public bool isFull = false;
    Image image;
    string spellHoldingName;

    public Image GetImage()
    {
        Transform child = selectedSlot.transform.GetChild(0);
        image = child.GetComponent<Image>();
        return image;
    }

    public void SetName(string spellAdded)
    {
        spellHoldingName = spellAdded;
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

    [Header("UI objects")]
    [SerializeField] Image spellSelected;
    [SerializeField] GameObject spellMenu;
    [SerializeField] Image menuButton;
    [SerializeField] TextMeshProUGUI currModifiyingSpellName;
    [SerializeField] GameObject[] menuParent = new GameObject[2];
    [SerializeField] GameObject[] menuLists = new GameObject[4];
    [SerializeField] Sprite[] buttonSelection = new Sprite[2];
    [SerializeField] SelectionSlot[] behaviorsButtons = new SelectionSlot[3];
    [SerializeField] SelectionSlot projectileButton;

    [Header("Script values")]

    // Reference to sprites in addon menu
    [SerializeField] Sprite[] projectileSprites = new Sprite[3];
    [SerializeField] Sprite[] behaviorSprites = new Sprite[2];
    Vector2 mousePos;
    int currHoldingSpellId = -1;
    int holdingSpellCount;
    bool swicthMenu;
    bool inSpellMenu;
    bool selectProjectile;
    bool selectBehavior;

    void Update()
    {
        if (inSpellMenu && (selectProjectile || selectBehavior))
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

    public void ResetHoldingSprite()
    {
        if (inSpellMenu && (selectProjectile || selectBehavior))
        {
            spellSelected.preserveAspect = false;
            Color trans = new Color(1, 1, 1, 0);
            spellSelected.color = trans;

            selectProjectile = false;
            selectBehavior = false;

            currHoldingSpellId = -1;
        }
    }

    #endregion

    #region UI buttons

    public void SelectProjectile(int id)
    {
        selectProjectile = true;

        spellSelected.color = Color.white;
        spellSelected.sprite = projectileSprites[id];
        spellSelected.preserveAspect = true;

        currHoldingSpellId = id;
    }

    public void SelectBehavior(int id)
    {
        selectBehavior = true;

        spellSelected.color = Color.white;
        spellSelected.sprite = behaviorSprites[id];
        spellSelected.preserveAspect = true;

        currHoldingSpellId = id;
    }

    public void SlotBehavior(int id)
    {
        if (!selectBehavior)
        {
            if(behaviorsButtons[id].isFull)
                SwitchToBHModifiers();
        }
        else
            DepositBehavior(id);
    }

    public void SlotProjectile()
    {
        if (!selectProjectile)
        {
            if(projectileButton.isFull)
                SwitchToPJModifiers();
        }
        else
            DepositProjectile();

    }

    void DepositBehavior(int id)
    {
        if (selectBehavior)
        {
            behaviorsButtons[id].GetImage().sprite = spellSelected.sprite;

            runeSlots.LoadBehavior(currHoldingSpellId);
            currModifiyingSpellName.text = runeSlots.GetName();

            behaviorsButtons[id].isFull = true;
            behaviorsButtons[id].selectedSlot.GetComponentInChildren<SpellGlowMask>().ActivateSpell();

            holdingSpellCount++;


            ResetHoldingSprite();
        }
    }

    void DepositProjectile()
    {
        if (selectProjectile)
        {
            projectileButton.GetImage().sprite = spellSelected.sprite;

            runeSlots.LoadProjectile(currHoldingSpellId);
            currModifiyingSpellName.text = runeSlots.GetName();

            projectileButton.isFull = true;
            projectileButton.selectedSlot.GetComponentInChildren<SpellGlowMask>().ActivateSpell();

            holdingSpellCount++;

            ResetHoldingSprite();
        }
    }

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

        menuParent[0].SetActive(false);
        menuParent[1].SetActive(true);

        menuLists[2].SetActive(true);
        menuLists[3].SetActive(false);
    }

    void SwitchToPJModifiers()
    {
        ResetHoldingSprite();

        menuParent[0].SetActive(false);
        menuParent[1].SetActive(true);

        menuLists[2].SetActive(false);
        menuLists[3].SetActive(true);
    }

    public void SwitchToBase()
    {
        ResetHoldingSprite();

        menuParent[0].SetActive(true);
        menuParent[1].SetActive(false);
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
