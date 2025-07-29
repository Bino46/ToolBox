using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "Spell", order = 1)]
public class SpellData : ScriptableObject
{
    public string spellName;
    public float speed;
    public float size;
    public float lifetime;
}
