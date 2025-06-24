using UnityEngine;

public class ShowHitPoiint : MonoBehaviour
{
    [SerializeField] GameObject hitPoint;
    Headbutt stateInfo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateInfo = GetComponentInParent<Headbutt>();
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
        }
        else
            hitPoint.SetActive(false);
    }

    void HideHitPoint()
    {
       hitPoint.SetActive(false); 
    }
}
