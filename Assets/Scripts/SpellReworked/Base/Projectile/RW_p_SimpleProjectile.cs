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

    #region System
    void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetProjectile();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (i_bounceCount > 0 && currBounceCount < i_bounceCount)
            Bounce(collision.contacts[0].normal);
        else
            modSpeed = 0;
    }
    #endregion

    #region Actions
    void Move()
    {
        //Simple movement
        _body.MovePosition(transform.position + (transform.forward * f_speed * modSpeed * Time.deltaTime));
    }

    void Bounce(Vector3 normal)
    {
        Vector3 bounceDir;
        Vector3 currDir = transform.forward;

        bounceDir = Vector3.Reflect(currDir, normal);
        transform.rotation = Quaternion.LookRotation(bounceDir);

        currBounceCount++;
    }

    void ResetProjectile()
    {
        modSpeed = 1;
        currBounceCount = 0;
    }
    #endregion
}
