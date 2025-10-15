using UnityEngine;
using UnityEngine.InputSystem;

public class RW_ShootSpell : MonoBehaviour
{
    [SerializeField] RW_SO_DataSpell data;
    public int projectileCount;
    public float offsetBetweenProjectiles;
    ControllerV2 controller;
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

            if (projectileCount > 1)
                radius = offsetBetweenProjectiles;

            for (int i = 0; i < projectileCount; i++)
            {
                float radians = 2 * 3.14f / projectileCount * i;

                offsetRotation = (controller.cameraPivot.transform.up * Mathf.Sin(radians) + controller.cameraPivot.transform.right * Mathf.Cos(radians)) * projectileCount;

                shootDir = controller.cameraPivot.transform.forward + offsetRotation * radius;

                SetProjectile(shootDir);
            }
        }
    }

    void SetProjectile(Vector3 dir)
    {
        int val = data.projectileType;

        if (val >= 0)
        {
            RW_Spell obj = GetProjectile(val);
            obj.SetProjectileDirectionAndPosition(dir, controller.cameraPivot.transform.position);
            obj.gameObject.SetActive(true);
        }
    }

    public void InitProjectile(int idPool)
    {
        foreach (GameObject obj in PoolManager._instance.poolList[idPool].objectList)
        {
            obj.GetComponent<RW_Spell>().InitSpell(data);
        }
    }

    RW_Spell GetProjectile(int type)
    {
        return PoolManager._instance.poolList[type].GetItem(data).GetComponent<RW_Spell>();
    }
}
