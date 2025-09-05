using UnityEngine;

public class ShootRay : MonoBehaviour
{
    LineRenderer line;
    [SerializeField] float lifetime;
    Pool pool;
    CompiledSpell currData;
    int bounces;
    int currBounce;
    Vector3 bounceStart;
    Vector3 bounceDir;

    #region System
    public void Init(CompiledSpell data, Vector3 startPos, Vector3 dir, Pool reference)
    {
        line = GetComponent<LineRenderer>();
        currData = data;
        pool = reference;

        bounceStart = startPos;
        bounceDir = dir;

        line.SetPosition(0, startPos);

        ReadModifiers();

        for (int i = 0; i <= bounces; i++)
        {
            FireRay(bounceStart, bounceDir, i + 1);
        }
    }

    void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Ray
    void FireRay(Vector3 startPos, Vector3 dir, int index)
    {
        RaycastHit hit;
        if (Physics.Raycast(startPos, dir, out hit, 1000))
            HitWall(startPos, hit.point, hit.normal, index);
        else
            ShowLine(dir * 1000, line.positionCount -1);
    }

    void HitWall(Vector3 startPos, Vector3 hitPoint, Vector3 normal, int index)
    {
        if (currBounce < bounces && bounces > 0)
        {
            currBounce++;
            line.positionCount++;
        }

        ShowLine(hitPoint, index);

        bounceStart = hitPoint;
        bounceDir = Vector3.Reflect(hitPoint - startPos, normal);

        MakeObject(hitPoint);
    }

    void ShowLine(Vector3 hitPoint, int index)
    {
        line.enabled = true;
        line.SetPosition(index, hitPoint);
    }

    void MakeObject(Vector3 spawnPos)
    {
        GameObject obj = pool.GetItem(currData);

        obj.transform.position = spawnPos;
        obj.GetComponent<Spell>().Init(currData);
    }

    #endregion

    #region Modifiers
    void ReadModifiers()
    {
        for (int i = 1; i < currData.followEffects.Count; i++)
        {
            if (currData.followEffects[i].currtType == AddedBehavior.dataType.Behaviour)
                break;
            else if (currData.followEffects[i].currtType == AddedBehavior.dataType.Modifier)
                ApplyProjectileModifier(currData.followEffects[i].id);
        }
    }

    void ApplyProjectileModifier(int modId)
    {
        switch (modId)
        {
            case 1:
                ShootSpell._instance.projectileFired = GetNumberOfProjectile();
                break;
            case 2:
                AddBounce();
                break;
        }
    }

    int GetNumberOfProjectile()
    {
        int val = 1;

        for (int i = 0; i < currData.followEffects.Count; i++)
        {
            if (currData.followEffects[i].currtType == AddedBehavior.dataType.Behaviour)
                break;
            else if (currData.followEffects[i].id == 1)
                val++;
        }
        return val;
    }

    void AddBounce()
    {
        bounces++;
    }
    #endregion
}
