using UnityEngine;
using UnityEngine.InputSystem;

public class ShootSpell : MonoBehaviour
{
    [SerializeField] SpellData currSpell;
    [SerializeField] GameObject prefab;
    public void Shoot(InputAction.CallbackContext ctx)
    {
        Quaternion rotation = Quaternion.Euler(GetComponent<ControllerV2>().viewRotation);
        GameObject newObject = Instantiate(prefab, transform.position, rotation);
        newObject.GetComponent<Spell>().Init(currSpell);
    }
}
