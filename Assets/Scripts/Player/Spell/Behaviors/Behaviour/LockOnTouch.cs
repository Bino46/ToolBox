using UnityEngine;

[CreateAssetMenu(fileName = "Spell Behavior", menuName = "LockOnTouch", order = 1)]
public class LockOnTouch : AddedBehavior
{
    [Header("SO values")]
    public bool b_lockOnTouch;
    public float f_timeBeforeDestruction;
}
