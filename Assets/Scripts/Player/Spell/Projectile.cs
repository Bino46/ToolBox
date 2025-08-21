using UnityEngine;

public class Projectile : MonoBehaviour
{
    CompliedSpell currData;
    Spell spell;
    Rigidbody body;
    [Header("Base value")]
    float speed;
    bool canGo;
    float currlifetime;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        spell = GetComponent<Spell>();
    }

    public void InitMovement(CompliedSpell data, SimpleProjectileData projData)
    {
        currData = data;

        //Base projectile stats
        speed = projData.f_speed;
        transform.localScale = Vector3.one * projData.f_size;
        currlifetime = projData.f_lifetime;
        body.mass = projData.f_mass;

        canGo = true;
        body.isKinematic = false;

        ReadModifiers();
    }

    #region Update methods
    void Update()
    {
        //bool for the projectile to move only when i want it to move (when active mostly)
        if (canGo)
            Move();
    }

    public void HandleLifetime()
    {
        currlifetime -= Time.deltaTime;

        if (currlifetime <= 0)
            spell.FullReset();
    }

    void Move()
    {
        //Simple movement
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }
    #endregion

    #region Modifiers

    void ReadModifiers()
    {
        for (int i = 1; i < currData.followEffects.Count; i++)
        {
            if (currData.followEffects[i].currtType == AddedBehavior.dataType.Modifier)
                ApplyProjectileModifier(currData.followEffects[i].id, i);
            else
                break;
        }
    }

    void ApplyProjectileModifier(int modId, int listId)
    {
        switch (modId)
        {
            case 0:
                spell.SetLockOnTouch(listId);
                break;
        }
    }

    public void LockProjectile(bool isLocked)
    {
        body.isKinematic = isLocked;
        canGo = !isLocked;
    }
    
    public void ExtendLifetime(float amount)
    {
        currlifetime += amount;
    }
    
    #endregion
    public void ResetMovement()
    {
        LockProjectile(true);
    }
}
