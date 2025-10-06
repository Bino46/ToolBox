using System.Collections.Generic;
using UnityEngine;

public class RW_Projectile : RW_MagicSlot
{
    private List<RW_Modifier> modList = new List<RW_Modifier>();
    public int i_bounceCount;
    
    public override void Init(RW_SO_DataSpell data, Vector3 startPos, Vector3 dir)
    {
        base.Init(data, startPos, dir);
        ApplyModifiers();
    }
    public void ApplyModifiers()
    { 

    }

    public virtual void ResetProjectile(){}
}
