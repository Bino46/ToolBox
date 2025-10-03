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
    public enum vfx
    {
        BlackHole = 0,
        Explosion = 1,
        Slash,
        None
    }
    public vfx visual;

    [Header("Modifiers section")]
    public float modStrengthValue = 1;
    public float modDurationValue = 1;
}
