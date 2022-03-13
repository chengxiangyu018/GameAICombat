using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class AgentMove : MonoBehaviour
{
    private NavMeshAgent _agent;

    private Vector3 targetPos;
    private Animator _animator;

    public bool debug;

    public Vector3 desireVelocity;
    // public static int ObstacleAreaMask =
    //     1 << NavMesh.GetAreaFromName("Area10") | 1 << NavMesh.GetAreaFromName("Area9");

    public Transform targetTrans;
    public Vector3 hitPos;
    [Header("Movement")] public float rotateSpeed = 180;

    public float stepHeight = 0.75f;

    /// <summary>
    /// 超过此旋转角度需要停止移动
    /// </summary>
    public float angleToRotate = 30f;

    public float arrivedDis = 0.1f;

    /// <summary>
    /// 向上修正速度（上台阶）
    /// </summary>
    public float upModifySpeed = 100;

    public Vector3 avoidVelocity;

    //代理半径
    public float avoidZoneRadius = 0.5f;
    public float avoidFactor = 1;
    public float avoidSpeed = 5;
    [Header("Ground")] public float GroundCheckStartOffset = 0.5f;

    public RaycastHit GroundHitInfo;

    /// <summary>
    /// 与地面在此范围内就认为在地面上（）
    /// </summary>
    public float GroundOffset = 0.3f;

    public float GroundChekRayLength = 10;
    public bool Grounded;

    public float Gravity = 9.8f;
    public Vector3 fallVelocity;
    public Vector3 fallOffset;
    public float disToGround;

    [Header("Avoid")]
    private AgentMove[] agentMoves;


    /// <summary>
    /// 地面摩擦力系数
    /// </summary>
    [Header("Hit")]
    public float groundDrag = 10;
    /// <summary>
    /// 空气阻力系数
    /// </summary>
    public float airDrag = 1;
    /// <summary>
    /// 其他速度（比如击飞等）
    /// </summary>
    public Vector3 otherVelocity;

    public Vector3 hitV;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.acceleration = 0;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agentMoves = GameObject.FindObjectsOfType<AgentMove>();
    }

    void Update()
    {
        ResetAgent();
        ResetAvoidVelocity();

        float deltaTime = Time.deltaTime;
        //_agent.
        if (debug)
            DrawNavigationPath(_agent.path, Color.green);


        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    if (_agent.Raycast(targetTrans.position, out var navMeshHit))
        //    {
        //        hitPos = navMeshHit.position;
        //        Debug.LogFormat($"<color=red>{navMeshHit.position}</color>");
        //    }
        //}
        if (Input.GetKeyDown(KeyCode.A))
        {

            SetOtherVelocity(hitV);
                Debug.LogFormat($"<color=red>hit</color>");
            
        }
        CheckGround();

        CalculateOtherVelocity(deltaTime);
        if (otherVelocity.magnitude<0.01f)
            CalculateAvoidVelocity();

        Movement(deltaTime);


        FinalMove(deltaTime);
        CheckArrived();
        MovementAni();
    }

    public void Move(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        _agent.isStopped = false;
        _agent.SetDestination(targetPos);
    }

    private void DrawNavigationPath(NavMeshPath path, Color color, float duration = 0)
    {
        if (duration == 0)
            duration = Time.deltaTime;
        if (path == null || path.corners.Length < 2)
        {
            return;
        }

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color, duration);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPos, 0.1f);
    }


    private void MovementAni()
    {
        float dx = Vector3.Dot(transform.right, desireVelocity);
        float dy = Vector3.Dot(transform.forward, desireVelocity);
        _animator.SetBool("move", desireVelocity.magnitude > 0.1f);
        _animator.SetFloat("velx", dx);
        _animator.SetFloat("vely", dy);
    }

    private void Movement(float deltaTime)
    {
        //Vector3 velocity = _agent.desiredVelocity;
        desireVelocity = _agent.desiredVelocity;
        Vector3 desireV2D = new Vector3(desireVelocity.x, 0, desireVelocity.z);
        Rotate(desireV2D, deltaTime);

        float angle = Vector3.Angle(desireV2D, transform.forward);
        if (angle <= angleToRotate)
        {
            desireVelocity = _agent.desiredVelocity;
        }
        else
        {
            desireVelocity = Vector3.zero;
        }
        if (avoidVelocity != Vector3.zero)
        {
            desireVelocity = avoidVelocity;
        }
        if (Grounded == false)
        {
            ApplyGravity(deltaTime);
        }
        else
        {
            ResetFallVelocity();
        }
    }

    private void FinalMove(float deltaTime)
    {
        Vector3 deltaMove = (desireVelocity + fallVelocity) * deltaTime;
        float deltaMoveDis = deltaMove.magnitude;
        Vector3 upModifyVelocity = Vector3.zero;
        if (Grounded)
        {
            float yOffset = GroundHitInfo.point.y - transform.position.y;
            //人物陷入地面（0.1为了解决误差问题）
            if (yOffset > 0.1f)
                upModifyVelocity = upModifySpeed * Vector3.up * deltaTime;
        }

        Debug.DrawRay(transform.position + 2 * Vector3.up, desireVelocity, Color.yellow);
        Debug.DrawRay(transform.position + 2 * Vector3.up, _agent.desiredVelocity, Color.red);
        //if (_agent.remainingDistance < deltaMoveDis && avoidVelocity == Vector3.zero)
        //{
        //    //transform.position = _agent.destination;
        //    //_agent.isStopped=true;
        //    Debug.Log(string.Format("<color=red>{0}arrived</color>", transform.gameObject.name));
        //}
        //else
        //{
            Vector3 targetPos = transform.position + (desireVelocity + otherVelocity + fallVelocity + upModifyVelocity) * deltaTime;
            if (_agent.Raycast(targetPos, out var navHit))
            {
                hitPos = navHit.position;
                Debug.Log("<color=red>hitPos</color>");
                transform.position = navHit.position;
            }
            else
            {
                transform.position = targetPos;
            }
        //}

    }

    private void CheckGround()
    {
        //需要注意台阶高度，stepHeight需要 >= NavmeshBuildSetting 里面的烘培参数stepHeight,不然会出现上台阶掉落情况
        Debug.DrawRay(transform.position + (stepHeight + 0.1f) * Vector3.up,
            (GroundCheckStartOffset + GroundOffset) * Vector3.down, Color.red);
        if (Physics.Raycast(transform.position + (stepHeight + 0.1f) * Vector3.up, Vector3.down, out GroundHitInfo,
            GroundChekRayLength, ~(1 << LayerMask.NameToLayer("Body"))))
        {
            disToGround = Vector3.Distance(transform.position, GroundHitInfo.point);

            float curStepH = GroundHitInfo.point.y - transform.position.y;
            //击中点在agent上方（有台阶）
            //0.1f因为有误差
            if (curStepH >= 0.1f)
                Grounded = true;
            else
            {
                Grounded = disToGround <= GroundOffset;
            }
        }
        else
        {
            Grounded = false;
            disToGround = 999;
        }

        // if (Grounded==false)
        // {
        //     UnityEditor.EditorApplication.isPaused = true;
        // }
    }

    /// <summary>
    /// 应用重力
    /// </summary>
    /// <param name="deltaTime"></param>
    private void ApplyGravity(float deltaTime)
    {
        fallVelocity += Gravity * Vector3.down;
        Vector3 nextPos = transform.position + fallVelocity * deltaTime;
        if (Physics.Linecast(transform.position + (stepHeight + 0.1f) * Vector3.up, nextPos, out var hit,
            ~(1 << LayerMask.NameToLayer("Body"))))
        {
            nextPos = hit.point;
        }

        fallOffset = nextPos - transform.position;
    }
    private void ResetAvoidVelocity()
    {
        avoidVelocity = Vector3.zero;
    }
    private void ResetFallVelocity()
    {
        fallVelocity = Vector3.zero;
    }

    private void ResetAgent()
    {
        _agent.nextPosition = transform.position;
    }

    private bool Rotate(Vector3 desireForward2D, float deltaTime)
    {
        Vector3 selfForward2d = transform.forward;
        selfForward2d.y = 0;
        float rotateAngle = Vector3.Angle(desireForward2D, selfForward2d);

        if (Mathf.Abs(rotateAngle) <= rotateSpeed * deltaTime)
        {
            if (desireForward2D.magnitude > 0.01f)
                transform.forward = desireForward2D.normalized;
            return false;
        }
        else
        {
            Vector3 selfRight2d = transform.right;
            selfRight2d.y = 0;
            float dirFactor = 1;

            float dotRightV = Vector3.Dot(desireForward2D, selfRight2d);

            if (dotRightV <= 0)
                dirFactor = -1;

            if (rotateAngle > angleToRotate)
            {
                // if (dirFactor > 0)
                // {
                //     MonsterBase.SetBool(MonsterAniParameter.TURN_R, true);
                //     MonsterBase.SetBool(MonsterAniParameter.TURN_L, false);
                // }
                // else
                // {
                //     MonsterBase.SetBool(MonsterAniParameter.TURN_R, false);
                //     MonsterBase.SetBool(MonsterAniParameter.TURN_L, true);
                // }
            }

            float deltaAngle = rotateSpeed * dirFactor * deltaTime;
            transform.Rotate(0, deltaAngle, 0, Space.Self);
            return true;
        }
    }

    private void CheckArrived()
    {
        if (_agent.pathPending)
            return;
        if (_agent.remainingDistance < arrivedDis)
        {
            _agent.isStopped = true;
        }
    }

    /// <summary>
    /// 目前仅避免agent重合
    /// </summary>
    private void CalculateAvoidVelocity()
    {
        foreach (var agent in agentMoves)
        {
            if (agent == this)
                continue;
            float avoidZone = Mathf.Max(_agent.radius, agent._agent.radius);
            Vector3 dirToAgent = agent.transform.position - transform.position;
            float angle = Vector3.Angle(dirToAgent, transform.forward);
            if (angle < 30)
            {
            }


            float disToAgent = Vector3.Distance(agent.transform.position, transform.position);
            if (disToAgent > 0.01f)
            {
                if (disToAgent <= avoidZone)
                {
                    avoidVelocity -= dirToAgent * ((avoidZone - disToAgent) / avoidZone);
                }
            }
            else//重合（随机一个避障速度）
            {
                float z = UnityEngine.Random.Range(-0.5f, 0.5f);
                float x = UnityEngine.Random.Range(-0.5f, 0.5f);
                avoidVelocity -= new Vector3(x, 0, z);
            }
        }
        avoidVelocity.y = 0;
        if (avoidVelocity != Vector3.zero)
        {
            avoidVelocity = avoidVelocity.normalized * avoidSpeed;
        }
    }


    /// <summary>
    /// 计算其他速度（比如击飞速度）
    /// </summary>
    /// <param name="deltaTime"></param>
    private void CalculateOtherVelocity(float deltaTime)
    {
        Vector3 horizontalDragVelocity = otherVelocity;
        horizontalDragVelocity.y = 0;
        
        //水平方向阻力
        if (horizontalDragVelocity.magnitude > 0.01f)
        {
            if (Grounded)
            {
                otherVelocity -= horizontalDragVelocity.normalized * groundDrag * deltaTime;
            }
            else
            {
                otherVelocity -= horizontalDragVelocity.normalized * airDrag * deltaTime;
            }
        }
        otherVelocity.x = Mathf.Max(otherVelocity.x , 0);
        //todo:解决接触到地面后，Y方向速度需要直接减少到0
        otherVelocity.y = Mathf.Max(otherVelocity.y - Gravity * deltaTime, 0);
        otherVelocity.z = Mathf.Max(otherVelocity.z, 0);
    }

    public void SetOtherVelocity(Vector3 v)
    {
        otherVelocity = v;
    }
}