using UnityEngine;

[CreateAssetMenu(fileName = " Spell Projectile", menuName = "SpellData", order = 1)]
public class SpellData : AddedBehavior
{
    [Header("SO values")]
    public float f_speed;
    public float f_size;
    public float f_lifetime;
    public float f_mass;
}
