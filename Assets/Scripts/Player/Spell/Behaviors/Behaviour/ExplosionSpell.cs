using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionSpell", menuName = "Spell Behavior/Explosion", order = 1)]
public class ExplosionSpell : AddedBehavior
{
    [Header("SO values")]
    public float f_explosionRadius;
    public float f_explosionStrength;
}
