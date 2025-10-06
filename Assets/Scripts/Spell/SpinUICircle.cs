using UnityEngine;

public class SpinUICircle : MonoBehaviour
{
    [SerializeField] float spinSpeed;
    [SerializeField] float moveSpeed;
    float timer;
    bool move;
    Vector3 newRotation;
    Vector3 newPosition;

    void Update()
    {
        newRotation.z += Time.deltaTime * spinSpeed;

        transform.localRotation = Quaternion.Euler(newRotation);

        if (move)
        {
            timer += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(transform.position, newPosition, timer);

            if (timer >= 1)
                move = false;
        }
    }

    public void MoveAt(Vector3 newPos)
    {
        move = true;
        timer = 0;
        newPosition = newPos;
    }

    public void ResetPos(Vector3 middlePos)
    {
        move = true;
        timer = 0;
        newPosition = middlePos;
    }
}
