using System.Collections.Generic;
using UnityEngine;

public class RW_b_Explosion : RW_Behavior
{
    public override void UseAbility(Vector3 pos)
    {
        List<Rigidbody> explosionBodyList = new List<Rigidbody>();

        //Simple explosion that pushes rigidbodies away
        RaycastHit[] ray = Physics.SphereCastAll(pos, baseDurationValue * modDurationValue, Vector3.one);
        if (explosionBodyList.Count == 0)
        {
            for (int i = 0; i < ray.Length; i++)
            {
                if (ray[i].transform.tag == "PhysicsObjects")
                {
                    explosionBodyList.Add(ray[i].collider.GetComponent<Rigidbody>());
                }
            }
        }

        //Debug.Log("explodes " + explosionBodyList.Count + " strength " + baseStrengthValue * modStrengthValue);

        foreach (Rigidbody obj in explosionBodyList)
        {
            obj.AddExplosionForce(baseStrengthValue * modStrengthValue, pos, baseDurationValue * modDurationValue);
        }

        if (modStrengthValue < 0)
            visual = vfx.BlackHole;
        else
            visual = vfx.Explosion;

        SummonVisualEffect(pos);
    }

    public override void ResetBehavior()
    {
        base.ResetBehavior();
    }
}
