using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;

public class ParkourAgent : Agent
{
    public Goal goal;
    Vector2 movementDir;
    float oldDistanceFromGoal;
    float distanceFromGoal;
    AIController controller;

    #region System
    void Start()
    {
        controller = GetComponent<AIController>();
    }
    void FixedUpdate()
    {
        //CalculateDistanceFromGoal();
    }
    #endregion

    #region Agent overrides
    public override void OnEpisodeBegin()
    {
        //goal.MoveGoal(false);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(goal.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("action");
        float moveX = actions.ContinuousActions[0];

        controller.Move(moveX);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = movementDir.x;
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



    #endregion
    //Inputs WASD
    public void MoveHorizontal(InputAction.CallbackContext ctx)
    {
        movementDir.x = ctx.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            controller.Jump();
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            controller.Dash();
    }
}