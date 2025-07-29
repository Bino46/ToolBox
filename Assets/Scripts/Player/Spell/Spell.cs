using UnityEngine;

public class Spell : MonoBehaviour
{
    [Header("Base value")]
    LayerMask mask;
    bool canGo;
    float speed;
    float b_lifetime;

    [Header("Mod value")]
    float currlifetime;

    void Start()
    {
        mask = LayerMask.GetMask("Self");
    }
    void Update()
    {
        if (canGo)
        {
            Move();
            Life();
        }
    }

    public void Init(SpellData data)
    {
        speed = data.speed;
        transform.localScale = Vector3.one * data.size;
        currlifetime = b_lifetime;

        canGo = true;
    }

    void Move()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    void Life()
    {
        currlifetime -= Time.deltaTime;

        if (currlifetime >= 0)
        {
            //! Pool
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        //! Pool
        if (other.gameObject.layer != mask)
            Destroy(gameObject);
    }
}
