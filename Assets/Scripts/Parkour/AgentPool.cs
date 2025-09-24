using System.Collections.Generic;
using UnityEngine;

public class AgentPool : MonoBehaviour
{
    [SerializeField] GameObject baseObject;
    [SerializeField] Goal goal;
    [SerializeField] int count;
    public List<GameObject> objectList = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            objectList.Add(MakeObject(i));
        }
    }
    public GameObject GetItem()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (!objectList[i].activeSelf)
            {
                objectList[i].SetActive(true);
                return objectList[i];
            }
        }

        //if all objects are busy, make another
        objectList.Add(MakeObject(objectList.Count));
        return objectList[objectList.Count - 1];

    }

    GameObject MakeObject(int name)
    {
        GameObject obj = Instantiate(baseObject, transform.position, transform.rotation);
        obj.transform.parent = gameObject.transform;
        obj.name = name.ToString();

        obj.GetComponent<ParkourAgent>().pool = this;
        obj.GetComponent<ParkourAgent>().goal = goal;

        return obj;
    }
}
