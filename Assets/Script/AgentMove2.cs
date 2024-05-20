using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AgentMove2 : MonoBehaviour
{
    public NavMeshAgent agent { get; private set; }
    public int id;

    public Vector2 pos2D => new Vector2(transform.position.x, transform.position.z);
    public Vector2 forward2D => new Vector2(transform.forward.x, transform.forward.z);

    public float maxSpeed => agent.speed;

    public Vector2 velocity2d => new Vector2(agent.velocity.x, agent.velocity.z); 
    public bool isTagged { get; set; }


    public SteeringController steeringController { get; private set; }
    public Vector2 desireVelocity;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InitAgent();
        steeringController = new SteeringController();
        steeringController.Init();
    }

    private void InitAgent()
    {
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
    
    public void ApplyVelocity()
    {
        var dv = new Vector3()
        {
            x = desireVelocity.x,
            y = agent.desiredVelocity.y,
            z = desireVelocity.y
        };
        //TODO：避障
        agent.velocity = dv;
    }
}

public class AgentParam
{
    
}