using System.Collections.Generic;
using UnityEngine;

public class RW_Projectile : RW_MagicSlot
{
    public List<RW_Modifier> modList = new List<RW_Modifier>();
    public int i_bounceCount; 
    public virtual void InitProjectile() { } 
}
