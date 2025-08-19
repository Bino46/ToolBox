using System.Collections.Generic;
using UnityEngine;

public class LoadSpellFromRune : MonoBehaviour
{
    [SerializeField] List<AddedBehavior> so_projectiles = new List<AddedBehavior>();
    [SerializeField] List<AddedBehavior> so_behaviors = new List<AddedBehavior>();
    [SerializeField] List<AddedBehavior> so_modifiers = new List<AddedBehavior>();
    [SerializeField] CompliedSpell currSpell;
    string lastLoaded;
    int slotProjectile = 0;

    void Start()
    {
        currSpell.followEffects.Clear();
    }

    public void LoadBehavior(int id)
    {
        lastLoaded = so_behaviors[id].name;
        currSpell.followEffects.Add(so_behaviors[id]);
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
            if (currSpell.followEffects[0].currtType != AddedBehavior.dataType.Projectile)
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
            Debug.Log("loop " + i);
            Debug.Log("behaviorSlot " + behaviorSlot + " searchIndex " + searchIndex + " currtype " + currSpell.followEffects[i].currtType);

            if (currSpell.followEffects[i].currtType == AddedBehavior.dataType.Behaviour)
            {
                if (behaviorSlot == searchIndex)
                    break;

                Debug.Log("spot behavior");
                behaviorSlot++;
            }

            if (currSpell.followEffects[i].currtType == AddedBehavior.dataType.Modifier)
            {
                Debug.Log("spot modifier");
                offset++;
            }
        }

        int newIndex = behaviorSlot + modifierIndex + offset;
        Debug.Log("offset " + offset + " new index " + newIndex + " searchIndex " + searchIndex);

        currSpell.followEffects.Insert(newIndex, so_modifiers[modifierType]);
    }

    public List<AddedBehavior> GetModifiersOnBehavior(int startIndex)
    {
        int behaviorSlot = startIndex + slotProjectile;

        List<AddedBehavior> modList = new List<AddedBehavior>();

        for (int i = behaviorSlot; i < currSpell.followEffects.Count; i++)
        {
            if (currSpell.followEffects[i].currtType == AddedBehavior.dataType.Modifier)
                modList.Add(currSpell.followEffects[i]);
            else
                break;
        }

        return modList;
    }
}
