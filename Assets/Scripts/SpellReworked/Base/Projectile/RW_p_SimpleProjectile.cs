using System;
using UnityEngine;

public class RW_p_SimpleProjectile : RW_Projectile
{
    Rigidbody _body;

    [Header("Base Values")]
    [SerializeField] float f_speed;
    [SerializeField] float f_bounceIntensity;
    [SerializeField] bool b_useGravity;
    [Header("Hidden Values")]
    int currBounceCount;
    float modSpeed = 1;
    bool canMove;

    #region System
    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        ResetProjectile();
    }

    void OnEnable()
    {
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
            
        transform.position = basePos;
    }

    public override void Init(RW_SO_DataSpell data)
    {
        base.Init(data);

        SortModifier();

        canMove = true;
    }

    void Update()
    {
        if(canMove)
            Move();
    }

    void OnCollisionEnter(Collision collision)
    {
        canMove = false;

        if (i_bounceCount > 0 && currBounceCount < i_bounceCount)
            Bounce(collision.GetContact(0).normal);
        else
        {
            modSpeed = 0;

            spellEffect.GetSignal(transform.position);
        }
    }

    void SortModifier()
    {
        for(int i = 0; i < modList.Length; i++)
        {
            if (modList[i] == null)
                return;

            ApplyModifier(i);
        }
    }

    void ApplyModifier(int i)
    {
        switch(modList[i].idx)
        {
            case 2:
                i_bounceCount = (int)MakeOperation(i_bounceCount, modList[i]);
                break;
            case 3:
                _body.useGravity = Convert.ToBoolean(MakeOperation(0, modList[i]));
                break;
        }
    }
    #endregion

    #region Actions
    void Move()
    {
        //Simple movement
        Vector3 moveDir = transform.position + (transform.forward * f_speed * modSpeed * Time.deltaTime);
        _body.MovePosition(moveDir);
    }

    void Bounce(Vector3 normal)
    {
        Vector3 bounceDir;
        Vector3 currDir = transform.forward;

        bounceDir = Vector3.Reflect(currDir, normal);
        bounceDir += Vector3.up * f_bounceIntensity;
        transform.rotation = Quaternion.LookRotation(bounceDir);

        currBounceCount++;
        canMove = true;
    }

    public override void ResetProjectile()
    {
        modSpeed = 1;
        currBounceCount = 0;
        canMove = true;

        _body.isKinematic = true;
        _body.isKinematic = false;
    }
    #endregion
}
