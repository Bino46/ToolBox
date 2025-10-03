using UnityEngine;

[CreateAssetMenu(fileName = "SlashSpell", menuName = "Spell Behavior/Slash", order = 1)]
public class SlashSpell : AddedBehavior
{
    [Header("SO values")]
    public float f_slashRadius;
    public float f_slashStrength;
}
