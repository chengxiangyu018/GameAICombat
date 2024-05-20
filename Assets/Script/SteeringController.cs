using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum FlockingType
{
    Separation=0,
    Alignment=1,
    Cohesion=2,
    Length
}

public class SteeringController
{
    private Dictionary<int, Func<AgentMove2, Vector2>> _flockingDic;
    private FlockingParam _flockingParam;
    public List<AgentMove2> neighbors = new List<AgentMove2>();

    public void Init()
    {
        _flockingDic = new Dictionary<int, Func<AgentMove2, Vector2>>()
        {
            {(int)FlockingType.Separation,Separation},
            {(int)FlockingType.Alignment,Alignment},
            {(int)FlockingType.Cohesion,Cohesion},
        };
        _flockingParam = new FlockingParam();
    }

    public Vector2 Separation(AgentMove2 self) 
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

    public Vector2 Alignment(AgentMove2 self)
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

    public Vector2 Cohesion(AgentMove2 self)
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

    public Vector2 CalculateSteeringForce(AgentMove2 self)
    {
        Vector2 steeringForce = Vector2.zero;
        for (int i = 0; i < (int)FlockingType.Length; i++)
        {
            var func = _flockingDic[i];
            var force = func(self);
            var remindForce = _flockingParam.maxForce - steeringForce.magnitude;
            if (remindForce < force.magnitude)
            {
                force *= (remindForce / force.magnitude);
                steeringForce += force;
                break;
            }

            steeringForce += force;
        }

        return steeringForce;
    }
}

[Serializable]
public class FlockingParam
{
    public byte flockingMask;
    public float maxForce = 99f;
}

    
    
