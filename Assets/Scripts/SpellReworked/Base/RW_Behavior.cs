using System;
using System.Collections.Generic;
using UnityEngine;

public class RW_Behavior : RW_MagicSlot
{
    public RW_SO_Behavior baseValue;
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
    public override void Init(RW_SO_DataSpell data, int id)
    {
        base.Init(data);
        ResetBehavior();

        ReadModifiers(data.behaviorAndModifiers[id].modListID);
    }

    void ReadModifiers(int[] indexes)
    {
        for (int i = 0; i < indexes.Length; i++)
        {
            if (indexes[i] == 0)
                break;

            modList[i] = dataSpell.bhModifiers[indexes[i]-1];
        }

        SortModifier();
    }

    void SortModifier()
    {
        for(int i = 0; i < modList.Length; i++)
        {
            if (modList[i] == null)
                return;

            ApplyModifier(i);
        }
    }

    void ApplyModifier(int i)
    {
        switch (modList[i].idx)
        {
            case 0:
                spellEffect.b_needRepeat = Convert.ToBoolean(MakeOperation(0, modList[i]));
                break;
            case 1:
                modStrengthValue = MakeOperation(modStrengthValue, modList[i]);
                break;
            case 2:
                modDurationValue = MakeOperation(modDurationValue, modList[i]);
                break;

            default:
                break;
        }
    }

    public void SummonVisualEffect(Vector3 pos)
    {
        GameObject obj = VFX_Manager._instance.GetVFX(visual);

        obj.transform.position = pos;
        obj.transform.localScale = Vector3.one * baseDurationValue * modDurationValue;
    }

    public void SummonVisualEffect(Vector3 pos, bool withChildren, float damage, float scale)
    {
        GameObject obj = VFX_Manager._instance.GetVFX(visual);

        obj.transform.position = pos;
        obj.transform.localScale = Vector3.one * scale;

        obj.GetComponent<VFX_Interface>().Show(1, 1, withChildren, Mathf.FloorToInt(damage));
    }
    
    public void ResetBehavior()
    {
        spellEffect.b_needRepeat = false;

        baseStrengthValue = baseValue.baseStrengthValue;
        baseDurationValue = baseValue.baseDurationValue;

        modStrengthValue = baseValue.modStrengthValue;
        modDurationValue = baseValue.modDurationValue;

        for (int i = 0; i < modList.Length; i++)
        {
            modList[i] = null;
        }
    }
}
