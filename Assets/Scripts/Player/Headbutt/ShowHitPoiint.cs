using UnityEngine;

public class ShowHitPoiint : MonoBehaviour
{
    [SerializeField] GameObject hitPoint;
    GrabObject checkGrab;
    Headbutt stateInfo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateInfo = GetComponentInParent<Headbutt>();
        checkGrab = GetComponent<GrabObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stateInfo.canSling == true && stateInfo.GetState() == Headbutt.States.charging)
            ShowHitPoint();
        else
            HideHitPoint();
    }

    void ShowHitPoint()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 2))
        {
            hitPoint.SetActive(true);
            hitPoint.transform.position = hit.point;

            checkGrab.GetObject(true, hit.collider.gameObject);
        }
        else
        {
            hitPoint.SetActive(false);

            checkGrab.GetObject(false, null);
        }
    }

    void HideHitPoint()
    {
       hitPoint.SetActive(false); 
    }
}
