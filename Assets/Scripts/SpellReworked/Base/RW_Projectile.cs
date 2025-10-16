using System.Collections.Generic;
using UnityEngine;

public class RW_Projectile : RW_MagicSlot
{
    public int i_bounceCount;
    public Vector3 dir;
    public Vector3 basePos;

    public override void Init(RW_SO_DataSpell data)
    {
        base.Init(data);
        ReadModifiers(data.projectileModifiers);
    }
    public void SetDirectionAndPosition(Vector3 newDir, Vector3 newPos)
    {
        dir = newDir;
        basePos = newPos;
    }
    void ReadModifiers(int[] indexes)
    {
        for(int i = 0; i < indexes.Length; i++)
        {
            if (indexes[i] == 0)
                return;

            modList[i] = dataSpell.pjModifiers[indexes[i] - 1];
        }
    }
    
    public virtual void ResetProjectile(){}
}
