using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "FullSpell", order = 1)]
public class FullSpell : ScriptableObject
{
    public string s_spellName;
    public List<AddedBehavior> followEffects = new List<AddedBehavior>();
}
