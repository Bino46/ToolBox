using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
class PrefabSlot
{
    public GameObject baseObject;
    public Button buttonFct;
    public int index;
    public enum type{Projectile, Behavior, Modifier};
    public type currType;

    public PrefabSlot(GameObject prefab, Transform parent, Sprite sprite, int idx)
    {
        baseObject = GameObject.Instantiate(prefab, parent);
        baseObject.name = idx.ToString();
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
    [SerializeField] SpinUICircle selectionIndicator;
    [SerializeField] Transform circleParent;
    [SerializeField] GameObject[] circleObjectList = new GameObject[6];
    [SerializeField] SpellGlowMask[] glow = new SpellGlowMask[6];
    [SerializeField] Transform modSlotsParent;
    List<PrefabSlot> modSlotsRef = new List<PrefabSlot>();
    [Header("Variables")]
    int currSelectedSlot = 0;
    bool[] isSlotFilled = new bool[6];
    int slotsAvailable;
    int slotFilledCount;

    void Start()
    {
        GenerateSlots();
        loadSpellData = GetComponent<RW_SpellLoadUI>();
        ChangeMenu(CurrMenu.pjMenu);
        ResetInterface();
    }

    #region Menu Managment
    public void ShowMenu(InputAction.CallbackContext ctx)
    {
        UIManager._instance.inMenu = !UIManager._instance.inMenu;
        spellCraftInterface.SetActive(UIManager._instance.inMenu);

        if(!spellCraftInterface.activeSelf)
            loadSpellData.InitSpell();
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
                break;
            case CurrMenu.pjMenu:
                projectileMenu.SetActive(true);
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

        selectionIndicator.ResetPos(circleSlots[0].transform.position);

        slotsAvailable = id;

        ChangeMenu(CurrMenu.pjMenu);
        currSelectedSlot = 0;

        ResetInterface();
        loadSpellData.ResetSpell(id);
    }

    void ResetInterface()
    {
        for (int i = 0; i < circleSlots.Length; i++)
        {
            circleSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = nullSprite;
            isSlotFilled[i] = false;
            glow[i].DesactivateSpell();
            glow[0].maxSize = 1.6f;
        }

        ResetModSlots();

        slotFilledCount = 0;

        loadSpellData.ResetData();
    }

    void ResetModSlots()
    {
        for(int i = 0; i < modSlotsRef.Count; i++)
        {
            modSlotsRef[i].baseObject.GetComponent<Image>().sprite = nullSprite;
            modSlotsRef[i].baseObject.SetActive(false);
        }
    }

    #endregion

    public void SelectCircleSlot(int slot)
    {
        if (currSelectedSlot != slot || modMenu.activeSelf)
        {
            modMenu.SetActive(false);
            behaviorMenu.SetActive(true);

            currSelectedSlot = slot;
            selectionIndicator.MoveAt(circleSlots[currSelectedSlot].transform.position);

            if (currSelectedSlot == 0)
                ChangeMenu(CurrMenu.pjMenu);
            else
                ChangeMenu(CurrMenu.bhMenu);
        }
        else if(isSlotFilled[slot])
        {
            modMenu.SetActive(true);
            behaviorMenu.SetActive(false);

            if (currSelectedSlot == 0)
                ChangeMenu(CurrMenu.pjMod);
            else
                ChangeMenu(CurrMenu.bhMod);

            DisplayActiveModifiers(currSelectedSlot);
        }
    }

    void FullGlowSpellCheck(int slot)
    {
        if (!isSlotFilled[slot])
        {
            isSlotFilled[slot] = true;
            slotFilledCount++;
            glow[slot].ActivateSpell();
        }

        if (slotFilledCount > slotsAvailable)
            glow[0].maxSize = 5;
    }

    void DisplayActiveModifiers(int selectedSlot)
    {
        GameObject child;
        for (int i = 0; i < modSlotsParent.childCount; i++)
        {
            child = modSlotsParent.GetChild(i).gameObject;
            child.GetComponent<Image>().sprite = nullSprite;
            child.SetActive(false);
        }

        modSlotsParent.GetChild(0).gameObject.SetActive(true);

        if (selectedSlot == 0)
            DisplayPjMods();
        else
            DisplayBhMods(selectedSlot);
    }
    void DisplayPjMods()
    {
        List<int> modList = loadSpellData.ReturnModList(0);

        for (int i = 0; i < modList.Count; i++)
        {
            modSlotsRef[i].baseObject.GetComponent<Image>().sprite = projectileModifierSprites[modList[i] - 1];
            modSlotsRef[i].baseObject.SetActive(true);
        }

        if (modList.Count < 16)
            modSlotsRef[modList.Count].baseObject.SetActive(true);
    }

    void DisplayBhMods(int selectedSlot)
    {
        List<int> modList = loadSpellData.ReturnModList(selectedSlot);

        for (int i = 0; i < modList.Count; i++)
        {
            modSlotsRef[i].baseObject.GetComponent<Image>().sprite = behaviorModifierSprites[modList[i] - 1];
            modSlotsRef[i].baseObject.SetActive(true);
        }

        if (modList.Count < 16)
            modSlotsRef[modList.Count].baseObject.SetActive(true);
    }
    

    #region Button Construction
    void SelectSlot(int id, int type)
    {
        loadSpellData.LoadIndex(currSelectedSlot, id);

        FullGlowSpellCheck(currSelectedSlot);

        switch (type)
        {
            case 0:
                circleSlots[currSelectedSlot].transform.GetChild(0).GetComponent<Image>().sprite = projectileSprites[id];
                break;
            case 1:
                circleSlots[currSelectedSlot].transform.GetChild(0).GetComponent<Image>().sprite = behaviorSprites[id];
                break;
        }
    }

    void SelectModifer(int id)
    {
        if (loadSpellData.CheckModQuantity(currSelectedSlot) < 16)
        {
            loadSpellData.LoadModifier(currSelectedSlot, id);
            DisplayActiveModifiers(currSelectedSlot);  
        }
    }

    void RemoveModifer(int id)
    {
        loadSpellData.RemoveModifier(currSelectedSlot, id);

        ResetModSlots();

        if (currSelectedSlot == 0)
            DisplayPjMods();
        else
            DisplayBhMods(currSelectedSlot);
    }
    
    #endregion

    #region Interface Generation
    [Button]
    void GenerateSlots()
    {
        CleanInterface();
        for (int i = 0; i < projectileSprites.Count; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, projectileMenu.transform, projectileSprites[i], i);
            obj.currType = PrefabSlot.type.Projectile;
            obj.buttonFct.onClick.AddListener(() => SelectSlot(obj.index, (int)obj.currType));
        }

        for (int i = 0; i < behaviorSprites.Count; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, behaviorMenu.transform, behaviorSprites[i], i);
            obj.currType = PrefabSlot.type.Behavior;
            obj.buttonFct.onClick.AddListener(() => SelectSlot(obj.index, (int)obj.currType));
        }

        for (int i = 0; i < projectileModifierSprites.Count; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, modProjectileMenu.transform, projectileModifierSprites[i], i);
            obj.currType = PrefabSlot.type.Modifier;
            obj.buttonFct.onClick.AddListener(() => SelectModifer(obj.index + 1));
        }

        for (int i = 0; i < behaviorModifierSprites.Count; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, modBehaviorMenu.transform, behaviorModifierSprites[i], i);
            obj.currType = PrefabSlot.type.Modifier;
            obj.buttonFct.onClick.AddListener(() => SelectModifer(obj.index + 1));
        }

        for (int i = 0; i < 16; i++)
        {
            PrefabSlot obj = new PrefabSlot(buttonPrefab, modSlotsParent.transform, nullSprite, i);
            obj.currType = PrefabSlot.type.Modifier;
            obj.buttonFct.onClick.AddListener(() => RemoveModifer(obj.index));

            modSlotsRef.Add(obj);
        }
    }
    [Button]
    void CleanInterface()
    {
        int val;

        val = projectileMenu.transform.childCount;
        for (int i = 0; i < val; i++)
        {
            DestroyImmediate(projectileMenu.transform.GetChild(0).gameObject);
        }

        val = behaviorMenu.transform.childCount;
        for (int i = 0; i < val; i++)
        {
            DestroyImmediate(behaviorMenu.transform.GetChild(0).gameObject);
        }

        val = modProjectileMenu.transform.childCount;
        for (int i = 0; i < val; i++)
        {
            DestroyImmediate(modProjectileMenu.transform.GetChild(0).gameObject);
        }

        val = modBehaviorMenu.transform.childCount;
        for (int i = 0; i < val; i++)
        {
            DestroyImmediate(modBehaviorMenu.transform.GetChild(0).gameObject);
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
