using UnityEngine;

public class RW_b_Delay : RW_Behavior
{
    public override void Init(RW_SO_DataSpell data)
    {
        base.Init(data);
        visual = vfx.None;
    }
    public override void UseAbility()
    {
        spellEffect.PauseTime += baseDurationValue * modDurationValue;
    }
}
