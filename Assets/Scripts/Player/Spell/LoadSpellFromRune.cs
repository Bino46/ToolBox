using System.Collections.Generic;
using UnityEngine;

public class LoadSpellFromRune : MonoBehaviour
{
    [SerializeField] List<AddedBehavior> so_projectiles = new List<AddedBehavior>();
    [SerializeField] List<AddedBehavior> so_behaviors = new List<AddedBehavior>();
    [SerializeField] CompliedSpell currSpell;

    public void LoadBehavior(int id)
    {
        currSpell.followEffects.Add(so_behaviors[id]);
    }

    public void LoadProjectile(int id)
    {
        if (currSpell.followEffects.Count > 0)
        {
            if (currSpell.followEffects[0].currtType != AddedBehavior.dataType.Projectile)
                currSpell.followEffects.Insert(0, so_projectiles[id]);
            else
                currSpell.followEffects[0] = so_projectiles[id];
        }
        else
            currSpell.followEffects.Add(so_projectiles[id]);
    }
}
