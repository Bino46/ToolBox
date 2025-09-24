using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;

public class MoveToGoalAgent : Agent
{
    public Goal goal;
    public float speed;
    Vector2 movementDir;
    float oldDistanceFromGoal;
    float distanceFromGoal;

    //System
    void FixedUpdate()
    {
        CalculateDistanceFromGoal();
    }

    #region Agent overrides
    public override void OnEpisodeBegin()
    {
        Debug.Log("end ep");
        goal.MoveGoal(false);
        RespawnAgent();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goal.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, moveY, 0) * Time.deltaTime * speed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = movementDir.x;
        continuousAction[1] = movementDir.y;
    }

    #endregion

    #region Helper methods
    public void WinReward()
    {
        AddReward(1f);
        EndEpisode();
    }

    public void FuckYou()
    {
        AddReward(-1f);
        EndEpisode();
    }

    void CalculateDistanceFromGoal()
    {
        distanceFromGoal = Vector3.Distance(transform.localPosition, goal.transform.localPosition);

        if (distanceFromGoal < oldDistanceFromGoal)
            AddReward(0.01f);
        else
            AddReward(-0.05f);

        oldDistanceFromGoal = distanceFromGoal;    
    }

    void RespawnAgent()
    {
        Vector3 newPos = Vector3.zero;

        newPos.x = Random.Range(-24, 24);
        newPos.y = Random.Range(0,20);

        transform.localPosition = newPos;
    }


    #endregion
    //Inputs WASD
    public void MoveHorizontal(InputAction.CallbackContext ctx)
    {
        movementDir.x = ctx.ReadValue<float>();
    }
    public void MoveVertical(InputAction.CallbackContext ctx)
    {
        movementDir.y = ctx.ReadValue<float>();
    }

}
