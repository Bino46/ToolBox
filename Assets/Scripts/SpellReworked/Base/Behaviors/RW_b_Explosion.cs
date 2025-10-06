using System.Collections.Generic;
using UnityEngine;

public class RW_b_Explosion : RW_Behavior
{
    List<Rigidbody> explosionBodyList = new List<Rigidbody>();

    public override void UseAbility(Vector3 pos)
    {
        //Simple explosion that pushes rigidbodies away, i keep the bodies in a list in case a Wait modifier loops the method
        RaycastHit[] ray = Physics.SphereCastAll(transform.position, baseDurationValue * modDurationValue, Vector3.one);
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

        // Debug.Log("explodes " + explosionBodyList.Count + " strength " + baseStrengthValue * modStrengthValue);

        foreach (Rigidbody obj in explosionBodyList)
        {
            obj.AddExplosionForce(baseStrengthValue * modStrengthValue, transform.position, baseDurationValue * modDurationValue);
        }

        if (modStrengthValue < 0)
            visual = vfx.BlackHole;
        else
            visual = vfx.Explosion;

        SummonVisualEffect(pos);
    }
}
