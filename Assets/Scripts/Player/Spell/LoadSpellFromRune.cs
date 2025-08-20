using System.Collections.Generic;
using UnityEngine;

public class LoadSpellFromRune : MonoBehaviour
{
    public List<AddedBehavior> so_projectiles = new List<AddedBehavior>();
    public List<AddedBehavior> so_behaviors = new List<AddedBehavior>();
    public List<AddedBehavior> so_modifiers = new List<AddedBehavior>();
    [SerializeField] CompliedSpell currSpell;
    string lastLoaded;
    int slotProjectile = 0;

    void Start()
    {
        currSpell.followEffects.Clear();
    }

    //Adds behavior to the list
    public void LoadBehavior(int id, int slot)
    {
        int slotBehavior = 0;
        lastLoaded = so_behaviors[id].name;

        if (slot + slotProjectile >= currSpell.followEffects.Count)
        {
            for (int i = currSpell.followEffects.Count; i < slot + slotProjectile; i++)
            {
                currSpell.followEffects.Add(null);
            }

            currSpell.followEffects.Add(so_behaviors[id]);
        }
        else
        {
            for (int i = slotProjectile; i < currSpell.followEffects.Count; i++)
            {
                if (currSpell.followEffects[i].currtType == AddedBehavior.dataType.Behaviour)
                    slotBehavior++;

                if (slotBehavior == slot + slotProjectile)
                {
                    Debug.Log("replace");
                    currSpell.followEffects[i] = so_behaviors[id];
                    break;
                }

            }
        }
    }

    public string GetName()
    {
        return lastLoaded;
    }

    public void LoadProjectile(int id)
    {
        lastLoaded = so_projectiles[id].name;

        if (currSpell.followEffects.Count > 0)
        {
            if (currSpell.followEffects[0] == null ||currSpell.followEffects[0].currtType != AddedBehavior.dataType.Projectile)
                currSpell.followEffects.Insert(0, so_projectiles[id]);
            else
                currSpell.followEffects[0] = so_projectiles[id];
        }
        else
            currSpell.followEffects.Add(so_projectiles[id]);

        slotProjectile = 1;
    }

    public void LoadBehaviorModifiers(int searchIndex, int modifierIndex, int modifierType)
    {
        int behaviorSlot = 0;
        int offset = 0;

        for (int i = slotProjectile; i < currSpell.followEffects.Count; i++)
        {
            if (currSpell.followEffects[i].currtType == AddedBehavior.dataType.Behaviour)
            {
                if (behaviorSlot == searchIndex)
                    break;

                behaviorSlot++;
            }

            if (currSpell.followEffects[i].currtType == AddedBehavior.dataType.Modifier)
                offset++;
        }

        int newIndex = behaviorSlot + modifierIndex + offset + slotProjectile;
        
        if (currSpell.followEffects.Count > newIndex && currSpell.followEffects[newIndex].currtType == AddedBehavior.dataType.Modifier)
            currSpell.followEffects[newIndex] = so_modifiers[modifierType];
        else
            currSpell.followEffects.Insert(newIndex, so_modifiers[modifierType]);
    }

    public List<AddedBehavior> GetModifiersOnBehavior(int searchIndex)
    {
        int behaviorSlot = 0;
        List<AddedBehavior> modList = new List<AddedBehavior>();


        for (int i = slotProjectile; i < currSpell.followEffects.Count; i++)
        {

            if (behaviorSlot == searchIndex + slotProjectile && currSpell.followEffects[i].currtType == AddedBehavior.dataType.Modifier)
                modList.Add(currSpell.followEffects[i]);

            if (currSpell.followEffects[i].currtType == AddedBehavior.dataType.Behaviour)
                behaviorSlot++;
        }

        return modList;
    }
}
