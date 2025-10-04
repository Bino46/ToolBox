using UnityEngine;

public class SlashHit : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float lifetime;
    [SerializeField] float parentLifetime;
    public int slashDamage;
    bool canHit;
    float size;
    float baseSize;
    float timer;
    
    void OnEnable()
    {
        baseSize = transform.localScale.x;
        size = baseSize;

        timer = 0;

        canHit = true;
    }

    void Update()
    {
        Attack();
    }

    void Attack()
    {
        timer += Time.deltaTime * speed;

        size = Mathf.Lerp(baseSize, baseSize * 3, timer);
        transform.localScale = Vector3.one * size;

        if (timer >= lifetime)
            canHit = false;

        if (timer >= parentLifetime)
            transform.GetComponentInParent<PoolObject>().ReturnToPool();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Entity" && canHit)
        {
            other.GetComponent<RPG_Entity>().TakeHit(slashDamage);
        }
    }
}
