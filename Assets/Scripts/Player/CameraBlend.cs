using UnityEngine;

public class CameraBlend : MonoBehaviour
{
    [SerializeField] Transform parentPos;
    [SerializeField] float minSpeed;
    Vector3 newPos;
    float timer;
    float distMult;
    float parentHeight;
    float height;

    void UpdateHeight()
    {
        distMult = Mathf.Abs(parentHeight - height);
        distMult = Mathf.Clamp(distMult, minSpeed, 100);

        timer += Time.deltaTime * distMult;

        newPos.y = Mathf.Lerp(height, parentHeight, timer);
    }

    // Update is called once per frame
    void Update()
    {
        newPos.x = parentPos.position.x;
        parentHeight = parentPos.position.y;
        newPos.z = parentPos.position.z;

        height = transform.position.y;

        if (parentHeight != height)
            timer = 0;

        UpdateHeight();

        transform.position = newPos;
        transform.rotation = parentPos.rotation;
    }
}
