using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.InputSystem;
class PrefabSlot
{
    GameObject baseObject;
    public Button buttonFct;
    public int index;

    public PrefabSlot(GameObject prefab, Transform parent, Sprite sprite, int idx)
    {
        baseObject = GameObject.Instantiate(prefab, parent);
        buttonFct = baseObject.GetComponent<Button>();

        baseObject.GetComponent<Image>().sprite = sprite;

        index = idx;
    }
}

public class RW_SpellCraftUI : MonoBehaviour
{
    RW_SpellLoadUI loadSpellData;
    [Header("Interface Generation")]
    [SerializeField] GameObject buttonPrefab;

    [Header("Sprites")]
    [SerializeField] Sprite nullSprite;
    [SerializeField] List<Sprite> projectileSprites = new List<Sprite>();
    [SerializeField] List<Sprite> behaviorSprites = new List<Sprite>();
    [SerializeField] List<Sprite> projectileModifierSprites = new List<Sprite>();
    [SerializeField] List<Sprite> behaviorModifierSprites = new List<Sprite>();
    [SerializeField] List<Sprite> interfaceSprites = new List<Sprite>();

    [Header("Menu List")]
    [SerializeField] GameObject spellCraftInterface;
    [SerializeField] GameObject baseMenu;
    [SerializeField] GameObject projectileMenu;
    [SerializeField] GameObject behaviorMenu;
    [SerializeField] GameObject modMenu;
    [SerializeField] GameObject modProjectileMenu;
    [SerializeField] GameObject modBehaviorMenu;
    [SerializeField] GameObject[] circleSlots = new GameObject[6];

    public enum CurrMenu { bhMenu = 0, pjMenu, bhMod, pjMod }
    [SerializeField] CurrMenu activeMenu;

    [Header("Interface objects")]
    [SerializeField] Image switchBaseMenuButton;
    [SerializeField] Transform circleParent;
    [SerializeField] GameObject[] circleObjectList = new GameObject[6];
    [Header("Variables")]
    int currSelectedSlot;
    int currCircle;

    void Start()
    {
        loadSpellData = GetComponent<RW_SpellLoadUI>();
        ChangeMenu(CurrMenu.pjMenu);
    }

    #region Menu Managment
    public void ShowMenu(InputAction.CallbackContext ctx)
    {
        UIManager._instance.inMenu = !UIManager._instance.inMenu;
        spellCraftInterface.SetActive(UIManager._instance.inMenu);
    }

    public void SwitchMenu()
    {
        if (activeMenu == CurrMenu.pjMenu)
            ChangeMenu(CurrMenu.bhMenu);
        else
            ChangeMenu(CurrMenu.pjMenu);
    }
    public void ChangeMenu(CurrMenu newMenu)
    {
        behaviorMenu.SetActive(false);
        projectileMenu.SetActive(false);
        modBehaviorMenu.SetActive(false);
        modProjectileMenu.SetActive(false);

        activeMenu = newMenu;

        if (activeMenu == CurrMenu.bhMenu || activeMenu == CurrMenu.pjMenu)
        {
            baseMenu.SetActive(true);
            modMenu.SetActive(false);
        }

        if (activeMenu == CurrMenu.bhMod || activeMenu == CurrMenu.pjMod)
        {
            baseMenu.SetActive(false);
            modMenu.SetActive(true);
        }

        switch (activeMenu)
        {
            case CurrMenu.bhMenu:
                behaviorMenu.SetActive(true);
                switchBaseMenuButton.sprite = interfaceSprites[1];
                break;
            case CurrMenu.pjMenu:
                projectileMenu.SetActive(true);
                switchBaseMenuButton.sprite = interfaceSprites[0];
                break;
            case CurrMenu.bhMod:
                modBehaviorMenu.SetActive(true);
                break;
            case CurrMenu.pjMod:
                modProjectileMenu.SetActive(true);
                break;

        }
    }

    public void SwitchCircle(int id)
    {
        for (int i = 0; i < circleObjectList.Length; i++)
        {
            circleObjectList[i].SetActive(false);
        }
        circleObjectList[id].SetActive(true);

        for (int i = 1; i < circleSlots.Length; i++)
        {
            circleSlots[i].SetActive(false);
        }
        for (int i = 0; i <= id; i++)
        {
            circleSlots[i].SetActive(true);
            circleSlots[i].transform.position = circleObjectList[id].transform.GetChild(i).position;
        }
    }

    #endregion

    public void SelectCircleSlot(int slot)
    {
        Debug.Log(slot);
        currSelectedSlot = slot;

        if (currSelectedSlot == 0)
            ChangeMenu(CurrMenu.pjMenu);
        else
            ChangeMenu(CurrMenu.bhMenu);
    }

    void SelectSlot(int id)
    {
        loadSpellData.LoadIndex(currSelectedSlot, id);
    }


    #region Interface Generation
    [Button]
    void GenerateSlots()
    {
        for (int i = 0; i < projectileSprites.Count; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, projectileMenu.transform, projectileSprites[i], i);
            obj.buttonFct.onClick.AddListener(() => SelectSlot(obj.index));
        }

        for (int i = 0; i < behaviorSprites.Count; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, behaviorMenu.transform, behaviorSprites[i], i);
            obj.buttonFct.onClick.AddListener(() => SelectSlot(obj.index));
        }

        for (int i = 0; i < projectileModifierSprites.Count; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, modProjectileMenu.transform, projectileModifierSprites[i], i);
            obj.buttonFct.onClick.AddListener(() => SelectSlot(obj.index));
        }

        for (int i = 0; i < behaviorModifierSprites.Count; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, modBehaviorMenu.transform, behaviorModifierSprites[i], i);
            obj.buttonFct.onClick.AddListener(() => SelectSlot(obj.index));
        }
    }
    [Button]
    void CleanInterface()
    {
        for (int i = 0; i < projectileMenu.transform.childCount; i++)
        {
            DestroyImmediate(projectileMenu.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < behaviorMenu.transform.childCount; i++)
        {
            DestroyImmediate(behaviorMenu.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < modProjectileMenu.transform.childCount; i++)
        {
            DestroyImmediate(modProjectileMenu.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < modBehaviorMenu.transform.childCount; i++)
        {
            DestroyImmediate(modBehaviorMenu.transform.GetChild(i).gameObject);
        }

    }

    [Button]
    void ResetCircleSlots()
    {
        for (int i = 0; i < circleParent.childCount; i++)
        {
            circleObjectList[i] = circleParent.GetChild(i).gameObject;
        }
    }
    #endregion
}
