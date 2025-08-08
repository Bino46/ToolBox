using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] ControllerV2 player;

    [Header("UI objects")]
    [SerializeField] Image spellSelected;
    [SerializeField] GameObject spellMenu;
    [SerializeField] Image menuButton;
    [SerializeField] GameObject[] menuLists = new GameObject[2];
    [SerializeField] Sprite[] buttonSelection = new Sprite[2];
    [SerializeField] Image[] behaviorsButtonsImages = new Image[3];
    [SerializeField] Image projectileButtonImage;

    [Header("Script values")]
    [SerializeField] Sprite[] projectileSprites = new Sprite[3];
    [SerializeField] Sprite[] behaviorSprites = new Sprite[2];
    Vector2 mousePos;
    bool swicthMenu;
    bool inSpellMenu;
    bool selectProjectile;
    bool selectBehavior;

    void Update()
    {
        if (inSpellMenu && (selectProjectile || selectBehavior))
        {
            spellSelected.transform.position = mousePos;
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
        }
    }

    #endregion

    #region UI actions
    public void SwitchMenu()
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

    public void SelectProjectile(int id)
    {
        selectProjectile = true;

        spellSelected.color = Color.white;
        spellSelected.sprite = projectileSprites[id];
        spellSelected.preserveAspect = true;
    }

    public void SelectBehavior(int id)
    {
        selectBehavior = true;

        spellSelected.color = Color.white;
        spellSelected.sprite = behaviorSprites[id];
        spellSelected.preserveAspect = true;
    }

    public void DepositBehavior(int id)
    {
        if (selectBehavior)
        {
            behaviorsButtonsImages[id].sprite = spellSelected.sprite;
            ResetHoldingSprite();
        }
    }

    public void DepositProjectile()
    {
        if (selectProjectile)
        {
            projectileButtonImage.sprite = spellSelected.sprite;
            ResetHoldingSprite();
        }
    }
    #endregion
}
