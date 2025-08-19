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
    public Sprite tex;
    public Material color;

    [Header("Modifiers section")]
    public float modStrengthValue = 1;
    public float modDurationValue = 1;
}
