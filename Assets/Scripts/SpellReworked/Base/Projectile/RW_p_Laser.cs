using UnityEngine;

public class RW_p_Laser : RW_Projectile
{
    LineRenderer _line;
    [Header("Hidden Values")]
    int currBounceCount;

    #region System
    void Awake()
    {
        _line = GetComponent<LineRenderer>();
    }
    void Start()
    {
        ResetProjectile();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (i_bounceCount > 0 && currBounceCount < i_bounceCount)
            Bounce(collision.contacts[0].normal);
    }
    #endregion

    #region Actions
    void Bounce(Vector3 normal)
    {
        Vector3 bounceDir;
        Vector3 currDir = transform.forward;

        bounceDir = Vector3.Reflect(currDir, normal);
        transform.rotation = Quaternion.LookRotation(bounceDir);

        currBounceCount++;
    }

    void ResetProjectile()
    {
        currBounceCount = 0;
    }
    #endregion
}
