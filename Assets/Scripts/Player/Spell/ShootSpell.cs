using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootSpell : MonoBehaviour
{
    public static ShootSpell _instance;
    [SerializeField] Pool simplePool;
    [SerializeField] Pool gravPool;
    [SerializeField] CompliedSpell currSpell;
    [SerializeField] float maxShootAngle;
    ControllerV2 controller;
    bool isGrav;
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
            Vector3 offsetRotation = Vector3.zero;

            float tempOffset = 0;

            if (projectileFired > 1)
                tempOffset = offsetBetweenProjectiles;
            
            for (int i = 0; i < projectileFired; i++)
                {
                    float radians = 2 * 3.14f / projectileFired * i;

                    offsetRotation.x = Mathf.Sin(radians) * tempOffset * projectileFired;
                    offsetRotation.y = Mathf.Cos(radians) * tempOffset * projectileFired;

                    Quaternion rotation = Quaternion.Euler(controller.viewRotation + offsetRotation);
                    GameObject newObject;

                    if (isGrav)
                        newObject = gravPool.GetItem();
                    else
                        newObject = simplePool.GetItem();

                    newObject.transform.position = controller.cameraPivot.transform.position;
                    newObject.transform.rotation = rotation;

                    newObject.GetComponent<Spell>().Init(currSpell);
                    newObject.SetActive(true);

                }
        }
    }

    public void ReadProjectile()
    {
        BaseProjectile proj = (BaseProjectile)currSpell.followEffects[0];

        switch (proj.id)
        {
            case 0:
                isGrav = false;
                break;
            case 1:
                isGrav = true;
                break;
        }
    }
}
