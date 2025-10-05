using System.Collections.Generic;
using UnityEngine;

public class RW_Behavior : RW_MagicSlot
{
    public List<RW_Modifier> modifiers = new List<RW_Modifier>();
    public virtual void UseAbility() { }
    public void InitBehavior()
    {
        //read modifiers
    }
}
