using UnityEditor.XR;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabObject : MonoBehaviour
{
    bool canGrab;
    bool isGrabbing;
    [SerializeField] Transform grabPoint;
    GameObject aimingObject;

    public void Grab(InputAction.CallbackContext ctx)
    {
        if (canGrab)
        {
            isGrabbing = true;
            aimingObject.transform.parent = grabPoint;
            aimingObject.GetComponent<Rigidbody>().useGravity = false;
            aimingObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void UnGrab(InputAction.CallbackContext ctx)
    {
        if (isGrabbing)
        {
            isGrabbing = false;
            aimingObject.transform.parent = null;
            aimingObject.GetComponent<Rigidbody>().useGravity = true;
            aimingObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void GetObject(bool grab, GameObject objectInfo)
    {
        if (!isGrabbing)
        {
            canGrab = grab;
            aimingObject = objectInfo; 
        }
    }

    void Update()
    {
        if (isGrabbing && aimingObject != null)
        {
            aimingObject.transform.position = grabPoint.position;
            aimingObject.transform.LookAt(grabPoint);
        }
    }
}
