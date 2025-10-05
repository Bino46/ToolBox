using UnityEngine;
using UnityEngine.InputSystem;

public class RW_ShootSpell : MonoBehaviour
{
    private RW_SO_DataSpell data;
    public int projectileCount;
    public float offsetBetweenProjectiles;
    ControllerV2 controller;
    void Start()
    {
        controller = GetComponent<ControllerV2>();
    }

    public void GetData(RW_SO_DataSpell newData)
    {
        data = newData;
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
        GameObject obj = GetProjectile();
    }

    GameObject GetProjectile()
    {
        return PoolManager._instance.poolList[data.projectileType].GetItem();
    }
}
