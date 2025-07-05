using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Headbutt : MonoBehaviour
{
    [SerializeField] Transform head;
    
    [Header("Variables")]
    public float chargeValue;
    [SerializeField] float chargeSpeed;
    [SerializeField] float maxCharge;
    [SerializeField] float thresholdSling;
    [SerializeField] float slingSpeed;
    [SerializeField] AnimationCurve slingValue;
    [SerializeField] float cancelSpeed;
    public enum States { idle, charging, slinging, charged, cancel }

    [Header("hidden values")]
    States currState;
    Animator animHead;
    public bool canSling { get; private set; }
    float timerSling;
    Vector3 headPos;

    void Start()
    {
        animHead = GetComponentInChildren<Animator>();
    }

    //Input
    public void ChargeHead(InputAction.CallbackContext ctx)
    {
        if (currState == States.idle)
        {
            chargeValue = 0;
            canSling = false;
            currState = States.charging;
        }
    }

    public void SlingHead(InputAction.CallbackContext ctx)
    {
        if (currState == States.charging)
        {
            currState = States.charged;
            timerSling = 0;
        }
    }

    //head related
    void LoadHit()
    {
        chargeValue -= Time.deltaTime * chargeSpeed;
        chargeValue = Mathf.Clamp(chargeValue, maxCharge, 0);
        headPos.z = chargeValue;

        AnimCharge(true);

        if (chargeValue <= thresholdSling)
            canSling = true;
    }

    void LaunchHit()
    {
        currState = States.slinging;
        chargeValue = 0;

        AnimCharge(false);

        timerSling += Time.deltaTime * slingSpeed;
        headPos.z = slingValue.Evaluate(timerSling);
    }

    void CancelHit()
    {
        currState = States.cancel;
        headPos.z += Time.deltaTime * cancelSpeed;

        AnimCharge(false);

        if (headPos.z > 0)
        {
            head.transform.position = Vector3.zero;
            currState = States.idle;
        }
    }

    void AnimCharge(bool charge)
    {
        if (charge)
        {
            animHead.SetLayerWeight(1, 1);
            animHead.SetBool("isCharging", true); 
        }
        else
        {
            animHead.SetLayerWeight(1, 0);
            animHead.SetBool("isCharging", false); 
        }
    }

    //Update
    void Update()
    {
        if (currState == States.charging)
            LoadHit();

        if (currState == States.charged || currState == States.cancel || currState == States.slinging)
        {
            if (canSling)
                LaunchHit();
            else
                CancelHit();
        }        

        if (currState == States.slinging && timerSling > slingValue.keys[2].time)
            currState = States.idle;

        head.transform.localPosition = headPos;
    }

    public States GetState()
    {
        return currState;
    }
}
