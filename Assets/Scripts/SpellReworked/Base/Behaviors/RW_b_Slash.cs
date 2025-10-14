using UnityEngine;

public class RW_b_Slash : RW_Behavior
{
    public override void Init(RW_SO_DataSpell data)
    {
        base.Init(data);
        visual = vfx.Slash;
    }

    public override void UseAbility(Vector3 pos)
    {
        SummonVisualEffect(pos, true, baseStrengthValue * modStrengthValue, baseDurationValue * modDurationValue);
    }
}
