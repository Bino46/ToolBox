using UnityEngine;
using System.Collections;

class Explosion
{
    public Vector3 SetExplosion(Vector3 pos, Vector3 origin, float radius, float force)
    {
        float distance = Vector3.Distance(origin, pos);

        if (distance > radius)
            return Vector3.zero;

        Vector3 pushValue = pos - origin;
        pushValue = Vector3.Normalize(pushValue) * force;

        float maxDistanceNormalized = distance / radius;

        pushValue = Vector3.Lerp(Vector3.zero, pushValue, maxDistanceNormalized);

        return pushValue;
    }
}
