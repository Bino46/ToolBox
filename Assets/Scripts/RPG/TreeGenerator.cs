using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] int seed;
    List<GameObject> allObjects = new List<GameObject>();
    [Header("Trunk")]
    [SerializeField] GameObject go_trunk;
    [SerializeField] float height;
    [SerializeField][Range(0, 1)] float heightRadiusRatio;

    [Header("Main Branches")]
    [SerializeField] GameObject go_branch;
    [SerializeField] int mainBranchCount;
    [SerializeField] float m_stepMinimumRotation;
    [SerializeField] Vector3[] m_minMaxBranchRotation = new Vector3[2];
    [SerializeField][Range(0, 2)] float m_minBranchHeight;
    [SerializeField][Range(0, 2)] float m_maxBranchHeight;
    [SerializeField][Range(0, 1)] float m_minBranchLength;
    [SerializeField][Range(0, 1)] float m_branchRadius;
    [SerializeField] AnimationCurve m_sizeReductionWithHeight;

    [Header("SmallBranches")]
    [SerializeField] int smallBranchCount;
    [SerializeField] float sizeReduction;
    [SerializeField] Vector3[] s_minMaxBranchRotation = new Vector3[2];

    [Header("SideBranch")]
    [SerializeField] int sideBranchCount;
    [SerializeField] int sideBranchLength;
    [SerializeField][Range(0, 1)] float sideBranchProba;
    [SerializeField] Vector3[] sideminMaxBranchRotation = new Vector3[2];

    [Header("Hidden values")]
    float radius;
    float currRotation;
    int currentBranchCount;

    [Button]
    void GenerateSeed()
    {
        seed = Random.Range(0, 150000);
        Random.InitState(seed);
    }

    [Button]
    void MakeTree()
    {
        GameObject parent = new GameObject();
        parent.transform.position = transform.position;
        allObjects.Add(parent);

        GameObject newTree = Instantiate(go_trunk, parent.transform);
        allObjects.Add(newTree);

        radius = height * heightRadiusRatio;
        newTree.transform.localScale = new Vector3(radius, height, radius);
        newTree.transform.position = new Vector3(transform.position.x, transform.position.y + height - 1, transform.position.z);

        for (int i = 0; i < mainBranchCount; i++)
        {
            MakeMainBranch(i);
        }
    }

    void MakeMainBranch(int index)
    {
        GameObject newBranch = Instantiate(go_branch, allObjects[0].transform);
        GameObject pivot = new GameObject(); // pivot for the next smaller branch

        pivot.transform.SetParent(newBranch.transform, true);
        pivot.transform.localPosition = Vector3.up;

        currRotation = m_stepMinimumRotation * index;
        Vector3 val;

        val.x = Random.Range(m_minMaxBranchRotation[0].x, m_minMaxBranchRotation[1].x);
        val.y = currRotation + Random.Range(m_minMaxBranchRotation[0].y, m_minMaxBranchRotation[1].y);
        val.z = Random.Range(m_minMaxBranchRotation[0].z, m_minMaxBranchRotation[1].z);

        Quaternion newRotation = Quaternion.Euler(val);
        newBranch.transform.rotation = newRotation;

        float jitter = Random.Range(height * m_minBranchHeight, height * m_maxBranchHeight);
        newBranch.transform.position = new Vector3(transform.position.x, jitter, transform.position.z);

        //Child (scaling the parent breaks the next steps)
        Transform childMesh = newBranch.transform.GetChild(0);

        val = Vector3.one * radius * m_branchRadius;

        //randomize a bit the length
        float rdmLength = Random.Range(0.5f * m_minBranchLength, m_minBranchLength);
        val.y += height * rdmLength;

        float evaluateHeight = Mathf.InverseLerp(height * m_minBranchHeight, height * m_maxBranchHeight, jitter);
        childMesh.localScale = val * m_sizeReductionWithHeight.Evaluate(evaluateHeight);

        val = Vector3.zero;
        val.y = childMesh.transform.localScale.y;
        childMesh.transform.localPosition = val; //This to avoid the big branch clipping through the tree

        val.y = childMesh.localScale.y + childMesh.localPosition.y;
        pivot.transform.localPosition = Vector3.up * val.y;

        allObjects.Add(newBranch);

        currentBranchCount = 0;

        MakeSideBranch(childMesh, pivot.transform);

        currentBranchCount = 0;
        if (smallBranchCount > 0)
            MakeSmallerBranch(childMesh, pivot.transform, smallBranchCount, false);
    }

    void MakeSmallerBranch(Transform previousScale, Transform parent, int length, bool isSide)
    {
        currentBranchCount++;

        GameObject newBranch = Instantiate(go_branch, parent, true);
        newBranch.name = "smol";

        GameObject newPivot = new GameObject();
        newPivot.transform.SetParent(newBranch.transform);

        Vector3 val = Vector3.zero;

        val.x = Random.Range(s_minMaxBranchRotation[0].x, s_minMaxBranchRotation[1].x);
        val.y = Random.Range(s_minMaxBranchRotation[0].y, s_minMaxBranchRotation[1].y);
        val.z = Random.Range(s_minMaxBranchRotation[0].z, s_minMaxBranchRotation[1].z);

        newBranch.transform.rotation = parent.rotation * Quaternion.Euler(val);

        Transform childMesh = newBranch.transform.GetChild(0);
        childMesh.transform.localScale = previousScale.localScale * sizeReduction;

        val = Vector3.zero;
        val.y = childMesh.transform.localScale.y;
        childMesh.transform.localPosition = val;

        val.y = childMesh.localScale.y + childMesh.localPosition.y;
        newPivot.transform.localPosition = Vector3.up * val.y;

        newBranch.transform.localPosition = Vector3.zero;

        // if(!isSide)
        //     MakeSideBranch(childMesh, newPivot.transform);

        if (currentBranchCount < length)
            MakeSmallerBranch(childMesh, newPivot.transform, length, false);
    }

    void MakeSideBranch(Transform previousScale, Transform parent)
    {
        float rdm = Random.Range(0.01f, 1);

        if (rdm <= sideBranchProba)
        {
            GameObject newBranch = Instantiate(go_branch, parent, true);
            newBranch.name = "side";

            GameObject newPivot = new GameObject();
            newPivot.transform.SetParent(newBranch.transform);

            Vector3 val = Vector3.zero;

            val.x = Random.Range(sideminMaxBranchRotation[0].x, sideminMaxBranchRotation[1].x);
            val.y = Random.Range(sideminMaxBranchRotation[0].y, sideminMaxBranchRotation[1].y);
            val.z = Random.Range(sideminMaxBranchRotation[0].z, sideminMaxBranchRotation[1].z);

            newBranch.transform.rotation = parent.rotation * Quaternion.Euler(val);

            Transform childMesh = newBranch.transform.GetChild(0);
            childMesh.transform.localScale = previousScale.localScale * sizeReduction;

            val = Vector3.zero;
            val.y = childMesh.transform.localScale.y;
            childMesh.transform.localPosition = val;

            val.y = childMesh.localScale.y + childMesh.localPosition.y;
            newPivot.transform.localPosition = Vector3.up * val.y;

            newBranch.transform.localPosition = Vector3.zero;

            MakeSmallerBranch(childMesh, newPivot.transform, sideBranchLength, true);
        }
    }
}
