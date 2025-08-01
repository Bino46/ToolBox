using System;
using UnityEngine;

public class AddedBehavior : ScriptableObject
{
    public enum dataType
    {
        Behaviour,
        Modifier,
        Projectile
    }

    [Header("Setup")]
    public dataType currtType;
    public int id;

    [Header("Modifiers section")]
    public float modValue = 1;
}
