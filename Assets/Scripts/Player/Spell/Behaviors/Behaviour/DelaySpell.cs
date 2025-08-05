using UnityEngine;

[CreateAssetMenu(fileName = "DelaySpell", menuName = "Spell Behavior/Delay", order = 1)]
public class DelaySpell : AddedBehavior
{
    [Header("SO values")]
    public float delayTime;
}
