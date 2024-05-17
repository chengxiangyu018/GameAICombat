
using UnityEngine;

public class AgentMgr
{
    private Dictionary<int, AgentMove2> agentMap = new Dictionary<int, AgentMove2>();
    public GameObject AgentPrefab;
    public int idToAllocate = 0;

    public AgentMove2 CreateAndInitAgent()
    {
        var agentMove = GameObject.Instantiate(AgentPrefab).GetComponent<AgentMove2>();
        agentMove.ID = idToAllocate;
        idToAllocate++;
        agentMap.Add(agentMove.ID,agentMove);
        return agentMove;
    }
}