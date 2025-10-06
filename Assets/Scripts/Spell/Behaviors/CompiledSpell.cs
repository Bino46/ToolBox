using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "CompliedSpell", menuName = "Complied Spell", order = 1)]
public class CompiledSpell : ScriptableObject
{
    public string s_spellName;
    public List<AddedBehavior> followEffects = new List<AddedBehavior>();
}
