using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class AgentMove2 : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Vector3 DebugVelocity;
    public int ID;
    public float AvoidSpeed;
    public void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        
    }

    public void Update()
    {
        
    }
}