using UnityEditor;
using UnityEngine;

public class Spell : MonoBehaviour
{
    LayerMask mask;
    PoolObject pool;
    Rigidbody body;

    [Header("Base value")]
    bool canGo;
    bool touch;
    float speed;
    float currlifetime;
    float deathTime;

    void Awake()
    {
        mask = LayerMask.GetMask("Self");
        body = GetComponent<Rigidbody>();
        pool = GetComponent<PoolObject>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (canGo)
        {
            Move();
            Life();
        }

        if (touch)
            TimedDestroy();
    }

    public void Init(SpellData data)
    {
        speed = data.speed;
        transform.localScale = Vector3.one * data.size;
        currlifetime = data.lifetime;
        deathTime = data.timeBeforeDestruction;

        body.isKinematic = false;
        canGo = true;
    }

    void Reset()
    {
        canGo = false;
        touch = false;

        body.isKinematic = true;

        pool.ReturnToPool();
    }

    #region SpellActions
    void Move()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    void Life()
    {
        currlifetime -= Time.deltaTime;

        if (currlifetime <= 0)
        {
            Reset();
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != mask)
            touch = true;
    }

    void TimedDestroy()
    {
        deathTime -= Time.deltaTime;
        if (deathTime <= 0)
        {
            Reset();
        }
    }

    #endregion
}
