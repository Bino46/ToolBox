using UnityEngine;

public class RPG_Entity : MonoBehaviour
{
    public string ent_name;
    public int hp;
    public int currHp { get; private set; }
    [SerializeField] float invFrames;
    float invTimer;
    void Start()
    {
        currHp = hp;
    }

    void Update()
    {
        if (invTimer > 0)
            invTimer -= Time.deltaTime;
    }

    public void TakeHit(int damage)
    {
        if (invTimer <= 0)
        {
            currHp -= damage;

            if (currHp <= 0)
                Kill();
    
            invTimer = invFrames;
        }
    }

    void Kill()
    {
        Destroy(gameObject);
    }
}
