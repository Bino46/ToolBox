using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] float hitStrength;
    [SerializeField] float hitRadius;
    Headbutt stateInfo;
    void Start()
    {
        stateInfo = GetComponentInParent<Headbutt>();
    }
    //Trigger knockback
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != null && other.gameObject.tag == "PhysicsObjects" && stateInfo.GetState() == Headbutt.States.slinging)
        {
            other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(hitStrength * 100, transform.position, hitRadius);
        }
    }
}
