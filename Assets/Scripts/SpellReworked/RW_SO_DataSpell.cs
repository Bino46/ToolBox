using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSpell", menuName = "Spell Data", order = 1)]
public class RW_SO_DataSpell : ScriptableObject
{
    [Header("Projectile")]
    public int projectileType;
    public List<int> projectileModifiers = new List<int>();
    
    [Header("Behaviors")]
    public int loadedBehaviorCount;
    public BehaviorWithModifierList[] behaviorAndModifiers = new BehaviorWithModifierList[5];
}
