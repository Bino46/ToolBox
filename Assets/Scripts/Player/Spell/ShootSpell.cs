using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootSpell : MonoBehaviour
{
    public static ShootSpell _instance;
    enum ProjectileType { Simple, Grav, Laser }
    ProjectileType currProjectile;
    [SerializeField] CompiledSpell currSpell;
    [SerializeField] GameObject baseRay;
    [SerializeField] float maxShootAngle;
    [SerializeField] Pool[] pools = new Pool[3];
    ControllerV2 controller;
    public int projectileFired;
    public float offsetBetweenProjectiles;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        controller = GetComponent<ControllerV2>();
    }

    public void Shoot(InputAction.CallbackContext ctx)
    {
        if (!UIManager._instance.inMenu)
        {
            if (currProjectile == ProjectileType.Laser)
                ShootRay();
            else
                ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        Vector3 offsetRotation = Vector3.zero;

        float tempOffset = 0;

        if (projectileFired > 1)
            tempOffset = offsetBetweenProjectiles;

        for (int i = 0; i < projectileFired; i++)
        {
            float radians = 2 * 3.14f / projectileFired * i;

            offsetRotation.y = Mathf.Sin(radians) * tempOffset * projectileFired;
            offsetRotation.z = Mathf.Cos(radians) * tempOffset * projectileFired;

            Quaternion rotation = Quaternion.Euler(controller.viewRotation + offsetRotation);
            GameObject newObject;

            newObject = GetFromPool();

            newObject.transform.position = controller.cameraPivot.transform.position;
            newObject.transform.rotation = rotation;

            newObject.GetComponent<Spell>().Init(currSpell);
        }
    }

    void ShootRay()
    {
        Vector3 offsetRotation = Vector3.zero;
        Vector3 shootDir = Vector3.zero;

        float tempOffset = 0;

        if (projectileFired > 1)
            tempOffset = offsetBetweenProjectiles;

        for (int i = 0; i < projectileFired; i++)
        {
            float radians = 2 * 3.14f / projectileFired * i;

            offsetRotation.x = Mathf.Sin(radians) * tempOffset * projectileFired;
            offsetRotation.y = Mathf.Cos(radians) * tempOffset * projectileFired;

            Quaternion rotation = Quaternion.Euler(offsetRotation);
            shootDir = rotation * controller.cameraPivot.transform.forward;

            ShootRay pew = Instantiate(baseRay.GetComponent<ShootRay>());

            pew.Init(currSpell, controller.cameraPivot.transform.position, shootDir, pools[2]);
        }
    }

    public void ReadProjectile()
    {
        BaseProjectile proj = (BaseProjectile)currSpell.followEffects[0];

        switch (proj.id)
        {
            case 0:
                currProjectile = ProjectileType.Simple;
                break;
            case 1:
                currProjectile = ProjectileType.Grav;
                break;
            case 2:
                currProjectile = ProjectileType.Laser;
                break;
        }
    }

    public void UpdateAllProjectiles(InputAction.CallbackContext ctx)
    {
        if (!UIManager._instance.inMenu && currSpell.followEffects.Count > 0)
        {
            switch (currProjectile)
            {
                case ProjectileType.Simple:
                    pools[0].UpdateProjectile(currSpell);
                    break;
                case ProjectileType.Grav:
                    pools[1].UpdateProjectile(currSpell);
                    break;
                case ProjectileType.Laser:
                    pools[2].UpdateProjectile(currSpell);
                    break;
            }
        }
    }

    GameObject GetFromPool()
    {
        switch (currProjectile)
        {
            case ProjectileType.Simple:
                return pools[0].GetItem(currSpell);

            case ProjectileType.Grav:
                return pools[1].GetItem(currSpell);

            case ProjectileType.Laser:
                return pools[2].GetItem(currSpell);
        }
        return null;
    }
}
