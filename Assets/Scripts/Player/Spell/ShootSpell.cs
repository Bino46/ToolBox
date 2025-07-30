using UnityEngine;
using UnityEngine.InputSystem;

public class ShootSpell : MonoBehaviour
{
    [SerializeField] Pool currPool;
    [SerializeField] SpellData currSpell;
    public void Shoot(InputAction.CallbackContext ctx)
    {
        GameObject newObject = currPool.GetItem();

        newObject.transform.position = transform.position;
        newObject.transform.rotation = transform.rotation;

        newObject.GetComponent<Spell>().Init(currSpell);
        newObject.SetActive(true);
    }
}
