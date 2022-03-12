using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickMove : MonoBehaviour
{
    private AgentMove _agentMove;
    void Start()
    {
        _agentMove = GetComponent<AgentMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out var m_HitInfo))
            {
                Debug.Log("m_HitInfo.point is " + m_HitInfo.point);
                //m_Agent.destination = m_HitInfo.point;
                //_path = m_Agent.path;
                _agentMove.Move(m_HitInfo.point);
                
            }
        }
    }
}
