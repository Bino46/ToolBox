using UnityEngine;

public class RW_p_Laser : RW_Projectile
{
    LineRenderer _line;
    [Header("Hidden Values")]
    int currBounceCount;
    LayerMask hitMask;
    Vector3 bounceStart;
    Vector3 bounceDir;

    #region System
    void Awake()
    {
        _line = GetComponent<LineRenderer>();
    }

    void Start()
    {
        ResetProjectile();
    }

    public override void Init(RW_SO_DataSpell data, Vector3 startPos, Vector3 dir)
    {
        base.Init(data, startPos, dir);

        hitMask = LayerMask.GetMask("Walls", "PhysicsObject", "Entity");
        bounceStart = startPos;
        bounceDir = dir;

        _line.SetPosition(0, startPos);

        for (int i = 0; i <= i_bounceCount; i++)
        {
            FireRay(bounceStart, bounceDir, i + 1);
        }
    }
    #endregion

    #region Actions

    void FireRay(Vector3 startPos, Vector3 dir, int index)
    {
        RaycastHit hit;
        if (Physics.Raycast(startPos, dir, out hit, 1000, hitMask))
            HitWall(startPos, hit.point, hit.normal, index);
        else
            ShowLine(dir * 25000, _line.positionCount - 1, true);
    }

    void HitWall(Vector3 startPos, Vector3 hitPoint, Vector3 normal, int index)
    {
        if (currBounceCount < i_bounceCount && i_bounceCount > 0)
        {
            currBounceCount++;
            _line.positionCount++;
        }
        else
            spellEffect.GetSignal(hitPoint);

        ShowLine(hitPoint, index, false);

        bounceStart = hitPoint;
        bounceDir = Vector3.Reflect(hitPoint - startPos, normal);
    }

    void ShowLine(Vector3 hitPoint, int index, bool miss)
    {
        _line.enabled = true;
        _line.SetPosition(index, hitPoint);

        if (miss && i_bounceCount > 0)
            spellEffect.GetSignal(_line.GetPosition(index-1));
    }

    void MakeObject(Vector3 spawnPos)
    {
        // GameObject obj = pool.GetItem(currData);

        // obj.transform.position = spawnPos;
        // obj.GetComponent<Spell>().Init(currData);
    }

    public override void ResetProjectile()
    {
        currBounceCount = 0;
        _line.positionCount = 2;
    }
    #endregion
}
