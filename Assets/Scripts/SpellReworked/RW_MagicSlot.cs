using System.Collections.Generic;
using UnityEngine;

public class RW_MagicSlot : MonoBehaviour
{
    public string slotName;
    public RW_Spell spellEffect { get; private set; }
    public virtual void Init(RW_SO_DataSpell data)
    {
        spellEffect = GetComponent<RW_Spell>();
    }
}
