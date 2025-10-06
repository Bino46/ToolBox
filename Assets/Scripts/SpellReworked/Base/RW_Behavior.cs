using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class RW_Behavior : RW_MagicSlot
{
    public List<RW_Modifier> modifiers = new List<RW_Modifier>();
    public enum vfx
    {
        BlackHole = 0,
        Explosion = 1,
        Slash,
        None
    }
    public vfx visual;
    public float baseStrengthValue;
    public float baseDurationValue;
    public float modStrengthValue;
    public float modDurationValue;
    public virtual void UseAbility(Vector3 pos) { }
    public void InitBehavior(RW_SO_DataSpell data)
    {
        ApplyModifiers();
    }

    public void ApplyModifiers(){}

    public void SummonVisualEffect(Vector3 pos)
    {
        GameObject obj = VFX_Manager._instance.GetVFX(visual);
        obj.transform.position = pos;
    }
}
