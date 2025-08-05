using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootSpell : MonoBehaviour
{
    [SerializeField] Pool currPool;
    [SerializeField] CompliedSpell currSpell;
    ControllerV2 controller;

    void Start()
    {
        controller = GetComponent<ControllerV2>();
    }

    public void Shoot(InputAction.CallbackContext ctx)
    {
        Quaternion rotation = Quaternion.Euler(controller.viewRotation);
        GameObject newObject = currPool.GetItem();

        newObject.transform.position = controller.cameraPivot.transform.position;
        newObject.transform.rotation = rotation;

        newObject.GetComponent<Spell>().Init(currSpell);
        newObject.SetActive(true);
    }
}
