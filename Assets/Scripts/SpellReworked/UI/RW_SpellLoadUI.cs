using System.Collections.Generic;
using UnityEngine;

public class RW_SpellLoadUI : MonoBehaviour
{

    [SerializeField] RW_ShootSpell shootSpell;
    [SerializeField] RW_SO_DataSpell dataSpell;

    public void LoadIndex(int slot, int id)
    {
        if (slot == 0)
            dataSpell.projectileType = id;
        else
            dataSpell.behaviorAndModifiers[slot - 1].behaviorID = id;
    }

    public void ResetSpell(int newSlotCount)
    {
        dataSpell.loadedBehaviorCount = newSlotCount;

        dataSpell.projectileType = -1;

        for (int i = 0; i < dataSpell.behaviorAndModifiers.Length; i++)
        {
            dataSpell.behaviorAndModifiers[i].behaviorID = -1;
        }
    }

    public void LoadModifier(int selectedSlot, int modId)
    {
        int modSlot = 0;
        if (selectedSlot == 0)
        {
            for (int i = 0; i < dataSpell.projectileModifiers.Length; i++)
            {
                if (dataSpell.projectileModifiers.Length > 0 && dataSpell.projectileModifiers[i] == 0)
                {
                    modSlot = i;
                    break;
                }
            }

            dataSpell.projectileModifiers[modSlot] = modId;
        }
        else
        {
            for (int i = 0; i < dataSpell.projectileModifiers.Length; i++)
            {
                if (dataSpell.behaviorAndModifiers[selectedSlot].modListID.Length > 0 && dataSpell.behaviorAndModifiers[selectedSlot].modListID[i] == 0)
                {
                    modSlot = i;
                    break;
                }
            }

            dataSpell.behaviorAndModifiers[selectedSlot].modListID[modSlot] = modId;
        }
    }

    public List<int> ReturnModList(int currSelectedSlot)
    {
        List<int> initializedMods = new List<int>();

        if (currSelectedSlot == 0)
        {
            for (int i = 0; i < dataSpell.projectileModifiers.Length; i++)
            {
                if (dataSpell.projectileModifiers[i] != 0)
                    initializedMods.Add(dataSpell.projectileModifiers[i]);
            }
        }
        else
        {
            for (int i = 0; i < dataSpell.behaviorAndModifiers[currSelectedSlot].modListID.Length; i++)
            {
                if (dataSpell.behaviorAndModifiers[currSelectedSlot].modListID[i] != 0)
                    initializedMods.Add(dataSpell.behaviorAndModifiers[currSelectedSlot].modListID[i]);
            }
        }
        return initializedMods;
    }
    
    public void ResetData()
    {
        dataSpell.projectileType = -1;

        for(int i = 0; i< dataSpell.projectileModifiers.Length;i++)
        {
            dataSpell.projectileModifiers[i] = 0;
        }

        for(int i = 0; i < dataSpell.behaviorAndModifiers.Length; i++)
        {
            dataSpell.behaviorAndModifiers[i].behaviorID = -1;

            for (int j = 0; j < dataSpell.behaviorAndModifiers[i].modListID.Length; j++)
                dataSpell.behaviorAndModifiers[i].modListID[j] = 0;
        }
    }
}
