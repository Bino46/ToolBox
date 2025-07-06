using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class PushTest : MonoBehaviour
{
    Rigidbody self;
    [SerializeField] float force;
    [SerializeField] float radius;
    List<Rigidbody> pushList = new List<Rigidbody>();

    void Start()
    {
        self = GetComponent<Rigidbody>();
    }

    [Button]
    void Push()
    {
        if (pushList.Count > 0)
        {
            foreach (Rigidbody body in pushList)
            {
                body.AddExplosionForce(force, transform.position, radius);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody newBody = other.GetComponent<Rigidbody>();

        if (newBody != null)
            pushList.Add(newBody);
    }
    void OnTriggerExit(Collider other)
    {
        Rigidbody newBody = other.GetComponent<Rigidbody>();

        if (newBody != null)
            pushList.Remove(newBody);
    }
}
