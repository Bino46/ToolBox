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
    }
}
