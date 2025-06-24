using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerV1 : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 currSpeed;
    [SerializeField] Vector2 maxSpeed;
    [SerializeField] float gravity;

    [Header("Raycast")]
    [SerializeField] float bottomStepReach;
    [SerializeField] float topStepReach;
    [SerializeField] float stepRayHeight;
    [SerializeField] float stepSpeed;
    [SerializeField] float maxFallDepthClip;
    [SerializeField] float bottomCollisionHeight;
    [SerializeField] float topCollisionHeight;

    [Header("Private")]
    private float localDirectionZ;
    private float localDirectionX;
    private Vector3 bottomPos;
    private Vector3 stepDetectionHeight;
    private Vector3 bottomCollisionHeightVector;
    private Vector3 topCollisionHeightVector;
    private bool touchStep;
    private bool stepping;
    private bool isGrounded;
    private float fallSpeed;

    public void MovePlayer(InputAction.CallbackContext ctx)
    {
        currSpeed.z = ctx.action.ReadValue<Vector2>().x * moveSpeed;
        currSpeed.x = -ctx.action.ReadValue<Vector2>().y * moveSpeed;
    }

    void ApplyMovement()
    {
        if (!isGrounded)
            fallSpeed -= gravity * Time.deltaTime;
        else
        {
            fallSpeed = 0;
        }
        
        currSpeed.y = fallSpeed;
        gameObject.transform.position += currSpeed;
    }

    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.localPosition, Vector3.down, out hit, 1.1f))
        {
            isGrounded = true;

            if (hit.distance <= maxFallDepthClip)
                gameObject.transform.position += Vector3.up * Time.deltaTime;
        }
        else
            isGrounded = false;
    }

    void CheckCollision()
    {
        localDirectionZ = 0.6f * Mathf.Sign(currSpeed.z);
        localDirectionX = -0.6f * Mathf.Sign(currSpeed.x);

        bottomCollisionHeightVector.x = gameObject.transform.position.x;
        bottomCollisionHeightVector.y = gameObject.transform.position.y - bottomCollisionHeight;
        bottomCollisionHeightVector.z = gameObject.transform.position.z;

        topCollisionHeightVector.x = gameObject.transform.position.x;
        topCollisionHeightVector.y = gameObject.transform.position.y + topCollisionHeight;
        topCollisionHeightVector.z = gameObject.transform.position.z;

        //bottom
        // Debug.DrawRay(bottomCollisionHeightVector, Vector3.forward * localDirectionZ, Color.violet, 0.1f);
        // Debug.DrawRay(bottomCollisionHeightVector, Vector3.left * localDirectionX, Color.green, 0.1f);

        if (Physics.Raycast(bottomCollisionHeightVector, Vector3.forward * localDirectionZ, 0.6f))
            currSpeed.z = 0;

        if (Physics.Raycast(bottomCollisionHeightVector, Vector3.left * localDirectionX, 0.6f))
            currSpeed.x = 0;

        //top
        // Debug.DrawRay(topCollisionHeightVector, Vector3.forward * localDirectionZ, Color.violet, 0.1f);
        // Debug.DrawRay(topCollisionHeightVector, Vector3.left * localDirectionX, Color.green, 0.1f);

        if (Physics.Raycast(topCollisionHeightVector, Vector3.forward * localDirectionZ, 0.6f))
            currSpeed.z = 0;

        if (Physics.Raycast(topCollisionHeightVector, Vector3.left * localDirectionX, 0.6f))
            currSpeed.x = 0;

    }

    void CheckStep()
    {
        bottomPos.x = gameObject.transform.position.x;
        bottomPos.y = gameObject.transform.position.y - stepRayHeight;
        bottomPos.z = gameObject.transform.position.z;

        // Debug.DrawRay(bottomPos, Vector3.forward * localDirectionZ, Color.blue, 0.1f);
        // Debug.DrawRay(bottomPos, Vector3.left * localDirectionX, Color.red, 0.1f);

        if (Physics.Raycast(bottomPos, Vector3.forward * localDirectionZ, bottomStepReach) || Physics.Raycast(bottomPos, Vector3.left * localDirectionX, bottomStepReach))
            touchStep = true;
    }

    void ApplyStep()
    {
        stepping = true;
        touchStep = false;

        float addYHeight = gameObject.transform.position.y - stepRayHeight;
        float diff = gameObject.transform.position.y - stepRayHeight;

        stepDetectionHeight.x = gameObject.transform.position.x;
        stepDetectionHeight.y = addYHeight;
        stepDetectionHeight.z = gameObject.transform.position.z;

        while (stepping == true)
        {
            if (Physics.Raycast(stepDetectionHeight, Vector3.forward * localDirectionZ, topStepReach) || Physics.Raycast(stepDetectionHeight, Vector3.left * localDirectionX, topStepReach))
            {
                addYHeight += stepSpeed * Time.deltaTime;
                stepping = false;
            }
        }

        gameObject.transform.position += new Vector3(0, addYHeight - diff, 0);
    }

    void FixedUpdate()
    {
        CheckGround();
        CheckCollision();

        if (currSpeed.x != 0 || currSpeed.z != 0)
        {
            CheckStep();

            if (touchStep)
                ApplyStep();
        }

        ApplyMovement();
    }
}
