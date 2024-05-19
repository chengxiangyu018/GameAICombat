
using System.Collections.Generic;
using UnityEngine;

public class AgentMgr:MonoBehaviour
{
    private Dictionary<int, AgentMove2> agentMap = new Dictionary<int, AgentMove2>();
    public GameObject AgentPrefab;
    public int idToAllocate = 0;
    public int neighborRange;
    public AgentMove2 CreateAndInitAgent()
    {
        var agentMove = GameObject.Instantiate(AgentPrefab).GetComponent<AgentMove2>();
        agentMove.ID = idToAllocate;
        idToAllocate++;
        agentMap.Add(agentMove.ID,agentMove);
        return agentMove;
    }

    public void UpdateNeighbors()
    {
        foreach (var self in agentMap.Values)
        {
            self.neighbors.Clear();
            foreach (var other in agentMap.Values)
            {
                if (self!=other && IsNeighbor(self, other))
                {
                    self.neighbors.Add(other);
                }
            }
        }
    }

    private bool IsNeighbor(AgentMove2 agent1,AgentMove2 agent2)
    {
        var range = agent1.agent.radius + agent2.agent.radius + neighborRange;
        return range * range >= Vector3.SqrMagnitude(agent1.transform.position - agent2.transform.position);
    }
}