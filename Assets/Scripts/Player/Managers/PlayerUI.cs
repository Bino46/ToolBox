using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] ControllerV2 player;
    [SerializeField] GameObject spellMenu;
    [SerializeField] Image menuButton;
    [SerializeField] GameObject[] menusSpell = new GameObject[2];
    [SerializeField] Sprite[] buttonSelection = new Sprite[2];
    bool swicthMenu;

    public void ShowMenu(InputAction.CallbackContext ctx)
    {
        spellMenu.SetActive(!spellMenu.activeSelf);
        player.LockPlayer(spellMenu.activeSelf);
    }

    public void SwitchMenu()
    {
        if (swicthMenu)
        {
            swicthMenu = false;

            menusSpell[0].SetActive(true);
            menusSpell[1].SetActive(false);

            menuButton.sprite = buttonSelection[0];
        }
        else
        {
            swicthMenu = true;

            menusSpell[0].SetActive(false);
            menusSpell[1].SetActive(true);

            menuButton.sprite = buttonSelection[1];
        }
    }
}
