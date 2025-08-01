using UnityEngine;

[CreateAssetMenu(fileName = "Spell Behavior", menuName = "DelayData", order = 1)]
public class DelaySpell : AddedBehavior
{
    [Header("SO values")]
    public float delayTime;
}
