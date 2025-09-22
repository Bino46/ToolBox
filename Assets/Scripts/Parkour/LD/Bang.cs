using UnityEngine;

public class Bang : MonoBehaviour
{
    LDManager parent;

    public void SetParent(LDManager newParent)
    {
        parent = newParent;
    }

    void OnTriggerEnter(Collider other)
    {
        parent.Respawn();
    }
}
