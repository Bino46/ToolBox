using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class RW_Behavior : RW_MagicSlot
{
    public RW_SO_Behavior baseValue;
    public List<RW_Modifier> modifiers = new List<RW_Modifier>();
    public enum vfx
    {
        BlackHole = 0,
        Explosion = 1,
        Slash,
        None
    }
    public vfx visual;
    public float baseStrengthValue { get; private set; }
    public float baseDurationValue { get; private set; }
    public float modStrengthValue { get; private set; }
    public float modDurationValue { get; private set; }

    
    public virtual void UseAbility() { }
    public virtual void UseAbility(Vector3 pos) { }
    public override void Init(RW_SO_DataSpell data)
    {
        base.Init(data);

        baseStrengthValue = baseValue.baseStrengthValue;
        baseDurationValue = baseValue.baseDurationValue;

        modStrengthValue = baseValue.modStrengthValue;
        modDurationValue = baseValue.modDurationValue;

        ApplyModifiers();
    }

    public void ApplyModifiers() { }

    public void SummonVisualEffect(Vector3 pos)
    {
        GameObject obj = VFX_Manager._instance.GetVFX(visual);

        obj.transform.position = pos;
        obj.transform.localScale = Vector3.one * baseDurationValue * modDurationValue;
    }
    
    public void SummonVisualEffect(Vector3 pos, bool withChildren, float damage)
    {
        GameObject obj = VFX_Manager._instance.GetVFX(visual);

        obj.transform.position = pos;
        obj.transform.localScale = Vector3.one * baseDurationValue * modDurationValue;

        obj.GetComponent<VFX_Interface>().Show(1,1,withChildren, Mathf.FloorToInt(damage)); 
    }
}
