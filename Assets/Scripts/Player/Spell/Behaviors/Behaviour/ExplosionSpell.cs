using UnityEngine;

[CreateAssetMenu(fileName = "Spell Behavior", menuName = "ExplosionData", order = 1)]
public class ExplosionSpell : AddedBehavior
{
    [Header("SO values")]
    public float f_explosionRadius;
    public float f_explosionStrength;
}
