using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public Pool pool;

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
