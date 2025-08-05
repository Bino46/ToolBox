using UnityEngine;

[CreateAssetMenu(fileName = "LockOnTouch", menuName = "Spell Behavior/LockOn", order = 1)]
public class LockOnTouch : AddedBehavior
{
    [Header("SO values")]
    public bool b_lockOnTouch;
    public float f_timeBeforeDestruction;
}
