using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public MoveToGoalAgent agent;
    [SerializeField] Transform newPos;
    [SerializeField] Vector2 minMaxDistance;
    [SerializeField] NotGoat[] hitWalls = new NotGoat[4];
    [SerializeField] Material winMat;
    [SerializeField] Material loseMat;


    void Start()
    {
        foreach (NotGoat goat in hitWalls)
        {
            goat.Setup(agent, this);
        }

        agent.goal = this;
    }

    void OnTriggerEnter(Collider other)
    {
        agent.WinReward();
        MoveGoal(true);
    }

    [Button]
    public void MoveGoal(bool win)
    {
        if (win)
        {
            foreach (NotGoat goat in hitWalls)
            {
                goat.GetComponent<MeshRenderer>().material = winMat;
            }
        }
        else
        {
            foreach (NotGoat goat in hitWalls)
            {
                goat.GetComponent<MeshRenderer>().material = loseMat;
            }
        }

        Vector3 newPos = Vector3.zero;

        newPos.x = Random.Range(-24,24);
        newPos.y = Random.Range(-20,0);

        transform.localPosition = newPos; 
    }
}
