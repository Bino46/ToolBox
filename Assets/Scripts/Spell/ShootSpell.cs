using UnityEngine;
using UnityEngine.InputSystem;

public class ShootSpell : MonoBehaviour
{
    public static ShootSpell _instance;
    enum ProjectileType { Simple, Grav, Laser }
    ProjectileType currProjectile;
    [SerializeField] CompiledSpell currSpell;
    [SerializeField] GameObject baseRay;
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
            Vector3 offsetRotation;
            Vector3 shootDir;

            float radius = 0;

            if (projectileFired > 1)
                radius = offsetBetweenProjectiles;

            for (int i = 0; i < projectileFired; i++)
            {
                float radians = 2 * 3.14f / projectileFired * i;

                offsetRotation = (controller.cameraPivot.transform.up * Mathf.Sin(radians) + controller.cameraPivot.transform.right * Mathf.Cos(radians)) * projectileFired;

                shootDir = controller.cameraPivot.transform.forward + offsetRotation * radius;

                if (currProjectile == ProjectileType.Laser)
                    MakeLaser(shootDir);
                else
                {
                    Quaternion newRotation = Quaternion.LookRotation(shootDir);
                    MakeProjectile(newRotation);
                }
            }
        }
    }

    void MakeProjectile(Quaternion rotation)
    {
        GameObject newObject;

        newObject = GetFromPool();

        newObject.transform.position = controller.cameraPivot.transform.position;
        newObject.transform.rotation = rotation;

        newObject.GetComponent<Spell>().Init(currSpell);
    }

    void MakeLaser(Vector3 dir)
    {
        ShootRay pew = Instantiate(baseRay.GetComponent<ShootRay>());

        pew.Init(currSpell, controller.cameraPivot.transform.position, dir, PoolManager._instance.poolList[2]);
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
                    PoolManager._instance.poolList[0].UpdateProjectile(currSpell);
                    break;
                case ProjectileType.Grav:
                    PoolManager._instance.poolList[1].UpdateProjectile(currSpell);
                    break;
                case ProjectileType.Laser:
                    PoolManager._instance.poolList[2].UpdateProjectile(currSpell);
                    break;
            }
        }
    }

    GameObject GetFromPool()
    {
        switch (currProjectile)
        {
            case ProjectileType.Simple:
                return PoolManager._instance.poolList[0].GetItem(currSpell);

            case ProjectileType.Grav:
                return PoolManager._instance.poolList[1].GetItem(currSpell);

            case ProjectileType.Laser:
                return PoolManager._instance.poolList[2].GetItem(currSpell);
        }
        return null;
    }
}
