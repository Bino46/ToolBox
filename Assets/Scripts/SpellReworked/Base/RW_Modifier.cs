using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Spell Modifier", order = 1)]
public class RW_Modifier : ScriptableObject
{
    public int idx;
    public float modifiedValue;
    public enum modType { Modify, Add, Multiply }
    public modType operation;
}
