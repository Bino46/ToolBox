using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using System;

public class ParkourAgent : Agent
{
    public Goal goal;
    public Transform spawnPoint;
    Vector2 movementDir;
    float oldDistanceFromGoal;
    float distanceFromGoal;
    AIController controller;
    public AgentPool pool;

    #region System
    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
    void Start()
    {
        controller = GetComponent<AIController>();
    }
    void FixedUpdate()
    {
        CalculateDistanceFromGoal();
    }
    #endregion

    #region Agent overrides
    public override void OnEpisodeBegin()
    {
        controller.Reset();
        transform.position = pool.transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(goal.transform.localPosition);
        sensor.AddObservation((int)controller.currState);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        controller.Move(moveX);

        int jump = actions.DiscreteActions[0];
        Debug.Log(actions.DiscreteActions[0]);

        if (jump == 1)
            controller.Jump();
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
    //Inputs
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