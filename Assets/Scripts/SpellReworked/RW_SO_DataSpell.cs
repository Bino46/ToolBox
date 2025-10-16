using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSpell", menuName = "Spell Data", order = 1)]
public class RW_SO_DataSpell : ScriptableObject
{
    [Header("Projectile")]
    public int projectileType;
    public int[] projectileModifiers = new int[16];
    
    [Header("Behaviors")]
    public int loadedBehaviorCount;
    public BehaviorWithModifierList[] behaviorAndModifiers = new BehaviorWithModifierList[5];

    [Header("Modifiers")]
    public List<RW_Modifier> pjModifiers = new List<RW_Modifier>();
    public List<RW_Modifier> bhModifiers = new List<RW_Modifier>();
}
