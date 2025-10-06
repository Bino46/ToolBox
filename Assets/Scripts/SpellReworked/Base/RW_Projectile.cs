using System.Collections.Generic;
using UnityEngine;

public class RW_Projectile : RW_MagicSlot
{
    public RW_Spell spellEffects;
    private List<RW_Modifier> modList = new List<RW_Modifier>();
    public int i_bounceCount;
    
    public virtual void InitProjectile(RW_SO_DataSpell data, Vector3 startPos, Vector3 dir)
    {
        spellEffects = GetComponent<RW_Spell>();
        ApplyModifiers();
    }
    public void ApplyModifiers() { }

    public virtual void ResetProjectile(){}
}
