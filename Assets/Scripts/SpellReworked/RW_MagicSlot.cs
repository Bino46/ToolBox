using UnityEngine;

public class RW_MagicSlot : MonoBehaviour
{
    public string slotName;
    public RW_Spell spellEffect { get; private set; }
    public RW_SO_DataSpell dataSpell { get; private set; }
    public RW_Modifier[] modList { get; private set; } = new RW_Modifier[16];
    public virtual void Init(RW_SO_DataSpell data)
    {
        spellEffect = GetComponent<RW_Spell>();
        dataSpell = data;
    }

    public float MakeOperation(float val, RW_Modifier mod)
    {
        switch (mod.operation)
        {
            case RW_Modifier.modType.Modify:
                return mod.modifiedValue;

            case RW_Modifier.modType.Add:
                return val + mod.modifiedValue;

            case RW_Modifier.modType.Multiply:
                return val * mod.modifiedValue;
        }

        return 0;
    }
}
