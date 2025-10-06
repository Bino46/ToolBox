using Unity.VisualScripting;
using UnityEngine;
public class RW_p_SimpleProjectile : RW_Projectile
{
    Rigidbody _body;

    [Header("Base Values")]
    [SerializeField] float f_speed;
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

    public override void Init(RW_SO_DataSpell data, Vector3 startPos, Vector3 dir)
    {
        base.Init(data, startPos, dir);

        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(dir);
        canMove = true;
    }

    void Update()
    {
        if(canMove)
            Move();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (i_bounceCount > 0 && currBounceCount < i_bounceCount)
            Bounce(collision.GetContact(0).normal);
        else
        {
            modSpeed = 0;
            canMove = false;
            spellEffect.GetSignal(transform.position);
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
        transform.rotation = Quaternion.LookRotation(bounceDir);

        currBounceCount++;
    }

    public override void ResetProjectile()
    {
        modSpeed = 1;
        currBounceCount = 0;
    }
    #endregion
}
