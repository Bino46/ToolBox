using UnityEngine;

public class Projectile : MonoBehaviour
{
    CompiledSpell currData;
    Spell spell;
    Rigidbody body;
    [Header("Base value")]
    float speed;
    bool canGo;
    float currlifetime;
    PhysicsMaterial bounceMat;

    void Awake()
    {
        spell = GetComponent<Spell>();
        bounceMat = GetComponent<SphereCollider>().material;
        body = GetComponent<Rigidbody>();   
    }

    public void InitMovement(CompiledSpell data, BaseProjectile projData)
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

        HandleLifetime();
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
        body.MovePosition(transform.position + (transform.forward * speed * Time.deltaTime));
    }

    #endregion

    #region Modifiers

    void ReadModifiers()
    {
        for (int i = 1; i < currData.followEffects.Count; i++)
        {
            if (currData.followEffects[i].currtType == AddedBehavior.dataType.Behaviour)
                break;
            else if(currData.followEffects[i].currtType == AddedBehavior.dataType.Modifier)
                ApplyProjectileModifier(currData.followEffects[i].id, i);
        }
    }

    void ApplyProjectileModifier(int modId, int listId)
    {
        switch (modId)
        {
            case 0:
                spell.SetLockOnTouch(listId);
                break;
            case 1:
                ShootSpell._instance.projectileFired = GetNumberOfProjectile();
                break;
            case 2:
                AddBounce(listId);
                break;
        }
    }

    int GetNumberOfProjectile()
    {
        int val = 1;

        for (int i = 0; i < currData.followEffects.Count; i++)
        {
            if (currData.followEffects[i].currtType == AddedBehavior.dataType.Behaviour)
                break;
            else if (currData.followEffects[i].id == 1)
                val++;
        }
        return val;
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

    void AddBounce(int listId)
    {
        if(bounceMat != null)
            bounceMat.bounciness += currData.followEffects[listId].modStrengthValue;
    }

    #endregion
    public void ResetMovement()
    {
        LockProjectile(true);
        bounceMat.bounciness = 0;     
    }
}
