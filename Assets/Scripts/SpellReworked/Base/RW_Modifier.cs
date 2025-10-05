using UnityEngine;

public class RW_Modifier : ScriptableObject
{
    public float modifiedValue;
    public enum modType { Modify, Add, Multiply }
    public modType operation;
}
