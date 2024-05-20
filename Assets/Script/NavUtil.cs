using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public static class NavUtil
{
    public static bool CalculatePath(Vector3 start,Vector3 end,NavMeshPath path)
    {
        return NavMesh.CalculatePath(start, end,NavMesh.AllAreas,path);
    }

    public const int RawRange=1000;
    public const int ColRange = 1000;
    public const int GridSize = 1;

    public static int HashPos(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / GridSize);
        int z = Mathf.FloorToInt(pos.z / GridSize);
        int res = x + z * 1000;
        return res;
    }
    
    /// <summary>
    /// 算个简单避障速度
    /// </summary>
    /// <param name="selfAgent"></param>
    /// <param name="agentMap"></param>
    /// <returns></returns>
    private static Vector3 CalculateAvoidVelocity(AgentMove2 selfAgent,Dictionary<int,AgentMove2>agentMap)
    {
        Vector3 res = Vector3.zero;
        foreach (var agentMove in agentMap.Values)
        {
            if (selfAgent.id == agentMove.id)
                continue;
            float avoidZone = Mathf.Max(agentMove.agent.radius, agentMove.agent.radius);
            Vector3 dirToAgent = agentMove.transform.position - agentMove.transform.position;
            
            float angle = Vector3.Angle(dirToAgent, agentMove.transform.forward);
            if (angle < 30)
            {
            }


            float disToAgent = Vector3.Distance(agentMove.transform.position, agentMove.transform.position);
            if (disToAgent > 0.01f)
            {
                if (disToAgent <= avoidZone)
                {
                    res -= dirToAgent * ((avoidZone - disToAgent) / avoidZone);
                }
            }
            else//重合（随机一个避障速度）
            {
                float z = UnityEngine.Random.Range(-0.5f, 0.5f);
                float x = UnityEngine.Random.Range(-0.5f, 0.5f);
                res -= new Vector3(x, 0, z);
            }
        }
        res.y = 0;
        if (res!= Vector3.zero)
        {
            res= res.normalized;
        }
        return res;
    }
}