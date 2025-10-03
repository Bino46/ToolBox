using UnityEngine;

public class PlanePLacement : MonoBehaviour
{
    Mesh mesh;
    [SerializeField] GameObject plane;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        foreach (Vector3 vec in mesh.normals)
        {
            Debug.Log(vec);
            GameObject obj = Instantiate(plane, transform);
            obj.transform.localPosition = vec;
        }
    }
}
