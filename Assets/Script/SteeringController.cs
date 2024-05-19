using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SteeringController
{
    public Vector2 Separation(AgentMove2 self, List<AgentMove2> neighbors) 
    {
        Vector2 steeringForce = Vector3.zero;
        foreach (var nb in neighbors)
        {
            if (self != nb && nb.isTagged)
            {
                Vector2 toAgent = self.pos2D - nb.pos2D;
                steeringForce += toAgent.normalized / toAgent.magnitude;
            }
        }

        return steeringForce;
    }

    public Vector2 Alignment(AgentMove2 self, List<AgentMove2> neighbors)
    {
        Vector2 averageHeading=Vector2.zero;
        int nbCount = 0;
        foreach (var nb in neighbors)
        {
            if (self != nb && nb.isTagged)
            {
                averageHeading += nb.forward2D;
                nbCount++;
            }

        }
        //不止一个nb,averageHeading取平均
        if (nbCount > 0)
        {
            averageHeading /= nbCount;
            averageHeading -= self.forward2D;
        }

        return averageHeading;
    }

    public Vector2 Cohesion(AgentMove2 self, List<AgentMove2> neighbors)
    {
        Vector2 centerOfMass = Vector2.zero;

        int nbCount = 0;

        foreach (var nb in neighbors)
        {
            if (self != nb && nb.isTagged)
            {
                centerOfMass += nb.pos2D;
                nbCount++;
            }
        }

        if (nbCount > 0)
        {
            centerOfMass /= nbCount;
        }

        var steeringForce = Seek(self,centerOfMass);
        return steeringForce;
    }

    public Vector2 Seek(AgentMove2 agent,Vector2 targetPos)
    {
        Vector2 desireV = (targetPos - agent.pos2D).normalized * agent.maxSpeed;
        Vector2 steeringForce = desireV - agent.velocity2d;
        return steeringForce;
    }

}
    
    
