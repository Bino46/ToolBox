using System.Collections.Generic;
using UnityEngine;

public class RW_Projectile : RW_MagicSlot
{
    private List<RW_Modifier> modList = new List<RW_Modifier>();
    public int i_bounceCount;
    public Vector3 dir;
    public Vector3 basePos;

    public override void Init(RW_SO_DataSpell data)
    {
        base.Init(data);
        ApplyModifiers();
    }
    public void SetDirectionAndPosition(Vector3 newDir, Vector3 newPos)
    {
        dir = newDir;
        basePos = newPos;
    }
    public void ApplyModifiers()
    { 

    }

    public virtual void ResetProjectile(){}
}
