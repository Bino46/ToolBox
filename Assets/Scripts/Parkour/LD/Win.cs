using UnityEngine;

public class Win : MonoBehaviour
{
    [SerializeField] LDManager manager;

    void OnTriggerEnter(Collider other)
    {
        manager.Respawn();
    }
}
