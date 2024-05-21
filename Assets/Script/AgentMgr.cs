
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
    public Vector3 groupTarget1;
    public Vector3 groupTarget2;
    
    private void Start()
    {
        for (int i = 0; i < count; i++)
        {
            int group = 1;
            if (i > count / 2.0f)
                group = 2;
            var agent = CreateAndInitAgent();
            agent.transform.position = Vector3.zero;
            agent.steeringController.groupId = group;
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
        ListenTargetChanged();
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

    private void ListenTargetChanged()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out var hitInfo))
            {
                groupTarget1 = hitInfo.point;
                SelectLeader(1);
            }
        }
        if (Input.GetMouseButtonDown(0)&&Input.GetKeyDown(KeyCode.LeftControl))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out var hitInfo))
            {
                groupTarget2 = hitInfo.point;
                SelectLeader(2);
            }
        }
    }

    private void SelectLeader(int groupId)
    {
        AgentMove2 leader = null;
        float minPos = float.MaxValue;

        foreach (var agent in _agentMap.Values)
        {
            if (agent.steeringController.groupId == groupId)
            {
                agent.leader = null;
                var dis = Vector3.Distance(agent.transform.position, groupTarget1);
                if (leader == null || dis < minPos)
                {
                    leader = agent;
                    minPos = dis;
                }

            }
        }
    
        foreach (var agent in _agentMap.Values)
        {
            if (agent.steeringController.groupId == groupId && agent != leader)
            {
                agent.leader = leader;
            }
        }
    }

    private bool IsNeighbor(AgentMove2 agent1,AgentMove2 agent2)
    {
        var range = agent1.agent.radius + agent2.agent.radius + neighborRange;
        return range * range >= Vector3.SqrMagnitude(agent1.transform.position - agent2.transform.position);
    }

}