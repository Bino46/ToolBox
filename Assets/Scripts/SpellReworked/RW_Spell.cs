using System.Collections.Generic;
using UnityEngine;

public class RW_Spell : MonoBehaviour
{
    private RW_SO_DataSpell data;
    private RW_MagicSlot projectile;
    private List<RW_MagicSlot> behaviors = new List<RW_MagicSlot>();

    public void Init(RW_SO_DataSpell newData)
    {
        data = newData;
    }

    //parse projectile

    //parse behavior
}
