using UnityEngine;

public class RW_p_Laser : RW_Projectile
{
    LineRenderer _line;
    [Header("Hidden Values")]
    int currBounceCount;
    LayerMask hitMask;
    Vector3 bounceStart;
    Vector3 bouceDir;

    #region System
    void Awake()
    {
        _line = GetComponent<LineRenderer>();
    }

    void Start()
    {
        ResetProjectile();
    }
    public override void Init(RW_SO_DataSpell data)
    {
        base.Init(data);

        SortModifier();

        hitMask = LayerMask.GetMask("Walls", "PhysicsObject", "Entity");
    }

    void SortModifier()
    {
        for(int i = 0; i < modList.Length; i++)
        {
            if (modList[i] == null)
                return;

            ApplyModifier(i);
        }
    }

    void ApplyModifier(int i)
    {
        switch(modList[i].idx)
        {
            case 2:
                i_bounceCount = (int)MakeOperation(i_bounceCount, modList[i]);
                break;
        }
    }

    void OnEnable()
    {
        bouceDir = dir;

        bounceStart = basePos;
        _line.SetPosition(0, basePos);

        for (int i = 0; i <= i_bounceCount; i++)
        {
            FireRay(bounceStart, bouceDir, i + 1);
        }
    }

    #endregion

    #region Actions

    void FireRay(Vector3 startPos, Vector3 newDir, int index)
    {
        RaycastHit hit;
        if (Physics.Raycast(startPos, newDir, out hit, 1000, hitMask))
            HitWall(startPos, hit.point, hit.normal, index);
        else
            ShowLine(newDir * 25000, _line.positionCount - 1, true);
    }

    void HitWall(Vector3 startPos, Vector3 hitPoint, Vector3 normal, int index)
    {
        ShowLine(hitPoint, index, false);
        if (currBounceCount < i_bounceCount && i_bounceCount > 0)
        {
            bounceStart = hitPoint;
            bouceDir = Vector3.Reflect(hitPoint - startPos, normal);

            currBounceCount++;
            _line.positionCount++;

        }
        else
            spellEffect.GetSignal(hitPoint);
    }

    void ShowLine(Vector3 hitPoint, int index, bool miss)
    {
        _line.enabled = true;
        _line.SetPosition(index, hitPoint);

        if (miss && i_bounceCount > 0)
            spellEffect.GetSignal(_line.GetPosition(index-1));
    }

    public override void ResetProjectile()
    {
        currBounceCount = 0;
        _line.positionCount = 2;
    }
    #endregion
}
