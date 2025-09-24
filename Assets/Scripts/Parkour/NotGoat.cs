using Unity.MLAgents;
using UnityEngine;

public class NotGoat : MonoBehaviour
{
    MoveToGoalAgent agent;
    Goal parent;

    public void Setup(MoveToGoalAgent newAgent, Goal newParent)
    {
        agent = newAgent;
        parent = newParent;
    }

    void OnTriggerEnter(Collider other)
    {
        parent.MoveGoal(false);
        agent.FuckYou();
    }
}
