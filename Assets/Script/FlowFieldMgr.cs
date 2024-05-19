using System.Collections.Generic;
using UnityEngine;

public class FlowFieldMgr
{
    private Dictionary<int, Dictionary<int, int>> gridMap = new Dictionary<int, Dictionary<int, int>>();
    
    
    public void RegisterToGridMap()
    {
        
    }
    
    public void UpdateGridMap(Vector3 leaderPos,Vector3 targetPos)
    {
        gridMap.Clear();
    }
}