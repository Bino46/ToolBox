using UnityEngine;

public class RW_SpellLoadUI : MonoBehaviour
{
    [SerializeField] RW_SO_DataSpell dataSpell;

    public void LoadIndex(int slot, int id)
    {
        if (slot == 0)
            dataSpell.projectileType = id;
        else
            dataSpell.behaviorAndModifiers[slot].behaviorID = id;
    }
}
