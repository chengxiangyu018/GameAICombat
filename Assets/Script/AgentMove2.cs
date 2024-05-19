using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = System.Random;

public class AgentMove2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public Vector3 DebugVelocity;
    public int ID;
    public float AvoidSpeed;

    public Vector2 pos2D => new Vector2(transform.position.x, transform.position.z);
    public Vector2 forward2D => new Vector2(transform.forward.x, transform.forward.z);

    public float maxSpeed => agent.speed;

    public Vector2 velocity2d => new Vector2(agent.velocity.x, agent.velocity.z); 
    public bool isTagged { get; set; }

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    public void Update()
    {
        
    }
    
}