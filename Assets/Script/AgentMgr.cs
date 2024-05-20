
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AgentMgr:MonoBehaviour
{
    private readonly Dictionary<int, AgentMove2> _agentMap = new Dictionary<int, AgentMove2>();
    public GameObject agentPrefab;
    private int _idToAllocate = 0;
    public int neighborRange;
    public int count;
    private void Start()
    {
        for (int i = 0; i < count; i++)
        {
            var agent = CreateAndInitAgent();
            agent.transform.position = Vector3.zero;
        }
    }

    private AgentMove2 CreateAndInitAgent()
    {
        var agentMove = GameObject.Instantiate(agentPrefab).GetComponent<AgentMove2>();
        agentMove.id = _idToAllocate;
        _idToAllocate++;
        _agentMap.Add(agentMove.id,agentMove);
        return agentMove;
    }
    
    private void Update()
    {
        UpdateNeighbors();
        UpdateSteeringForce();
        ApplyVelocity();
    }
    private void UpdateNeighbors()
    {
        foreach (var self in _agentMap.Values)
        {
            self.steeringController.neighbors.Clear();
            foreach (var other in _agentMap.Values)
            {
                if (self!=other && IsNeighbor(self, other))
                {
                    self.steeringController.neighbors.Add(other);
                }
            }
        }
    }

    private void UpdateSteeringForce()
    {
        foreach (var self in _agentMap.Values)
        {
            self.desireVelocity = self.steeringController.CalculateSteeringForce(self);
        }
    }
    private void ApplyVelocity()
    {
        foreach (var self in _agentMap.Values)
        {
            self.ApplyVelocity();
        }
    }
    private bool IsNeighbor(AgentMove2 agent1,AgentMove2 agent2)
    {
        var range = agent1.agent.radius + agent2.agent.radius + neighborRange;
        return range * range >= Vector3.SqrMagnitude(agent1.transform.position - agent2.transform.position);
    }

}