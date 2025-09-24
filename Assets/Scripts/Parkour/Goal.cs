using UnityEngine;

public class Goal : MonoBehaviour
{
    public ParkourAgent agent;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Agent"))
            agent.WinReward();
    }

}
