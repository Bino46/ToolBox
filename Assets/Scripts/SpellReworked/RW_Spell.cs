using System.Collections.Generic;
using UnityEngine;

public class RW_Spell : ScriptableObject
{
    public RW_MagicSlot projectile;
    public List<RW_MagicSlot> behaviors = new List<RW_MagicSlot>();
}
