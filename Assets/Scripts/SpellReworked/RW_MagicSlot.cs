using System.Collections.Generic;
using UnityEngine;

public class RW_MagicSlot : MonoBehaviour
{
    public string slotName;
    public Sprite icon;
    public enum SlotType { Projectile, Behavior, Modifier }
    public SlotType slotType;
}
