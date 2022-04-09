using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TestController : MonoBehaviour
{
    public UnitManager unitManager;
    public Animator animator;
    public Vector3 targetPos;
    public NavMeshAgent agent;
    public AnimationClip attackAnimation;
    public int avoidancePriority;

    public float stackTimer = 2.0f;
    float prevDis = 0.0f;

    float animationTime;
    float autoCD = 0.0f;

    //temp
    bool inited = false;

    public bool targetArrived = true;

    public void Init(Vector3 rallyPoint)
    {
        targetPos = rallyPoint;
        inited = true;
    }
    void Start()
    {
        unitManager = GetComponent<UnitManager>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        animationTime = attackAnimation.length;
        if (!inited) targetPos = transform.position;
    }
    public void LogicUpdate(float logicLen)
    {
        if (animator.GetInteger("states") == 3 || agent.enabled == false) return; // Dead.
        animator.SetFloat("speed", agent.velocity.magnitude);

        if (animator.GetInteger("states") == 2) // Attacking; stay hold;
        {
            targetPos = transform.position;
        }
        else if (unitManager.target != null && unitManager.target.tag == "Unit") // Attempt to attack a unit, move to the position and then attack.
        {
            Vector3 distance = unitManager.target.transform.position - transform.position;
            if (!unitManager.target.GetComponent<UnitManager>().IsAlive())
            {
                unitManager.target = null;
            }
            else
            {
                if (autoCD == 0.0f && distance.magnitude <= unitManager.attackRange + unitManager.target.GetComponent<UnitManager>().radius)
                {
                    transform.forward = Vector3.Normalize(distance);
                    //Debug.Log(distance.magnitude + ", " + transform.forward);
                    OneAttack();
                    targetPos = transform.position;
                }
                else
                {
                    if (targetArrived)
                    {
                        //targetPos = unitManager.GetNearestLoc();
                        targetPos = unitManager.target.transform.position;
                    }
                }
            }
        }
        else if (unitManager.target != null && unitManager.target.tag == "Building") // Attack building, similar as above.
        {
            Vector3 distance = unitManager.target.transform.position - transform.position;
            if (!unitManager.target.GetComponent<BuildingManager>().IsAlive)
            {
                unitManager.target = null;
            }
            else if (autoCD == 0.0f)
            {

                if (autoCD == 0.0f && (distance.magnitude <= unitManager.attackRange + unitManager.target.GetComponent<BuildingManager>().width / 2 || unitManager.touchTarget))
                {
                    transform.forward = Vector3.Normalize(distance);
                    //Debug.Log(distance.magnitude + ", " + transform.forward);
                    OneAttack();
                    targetPos = transform.position;
                }
                else
                {
                    if (targetArrived)
                    {
                        //targetPos = unitManager.GetNearestLoc();
                        targetPos = unitManager.target.transform.position;
                    }
                }
            }
        }
        SetDestination(); // Set the destination and move to there.
        UpdateAutoCD(logicLen);
        IsMoving();
    }
    public void RenderUpdate(float renderLerpValue)
    {
        
    }

    public void SetTargetPos(Vector3 point)
    {
        targetPos = point;
        targetArrived = false;
    }
    void SetDestination()
    {
        Vector3 a = new Vector3(targetPos.x, 0, targetPos.z);
        Vector3 b = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 distance = a - b;
        //Debug.Log(targetPos);
        //Debug.Log(transform.position);
        if(distance.magnitude > 1.0f)
        {
            targetArrived = false;
            agent.avoidancePriority = 50;
        }
        else
        {
            targetArrived = true;
            agent.avoidancePriority = 40;
        }
        agent.destination = targetPos; // Unity's built-in NavMeshAgent processes this position and manages the movement.
    }
    public void AdjestTargetPos()
    {

        Vector3 distance = targetPos - transform.position;
        Debug.Log(targetPos);
        Debug.Log(transform.position);
        Debug.Log(distance);
        Debug.Log(distance.magnitude);
    }
    void IsMoving()
    {
        if (agent.velocity.magnitude < 1)
        {
            unitManager.moving = false;
        }
        else
        {
            unitManager.moving = true;
        }
    }
    void UpdateAutoCD(float logicLen)
    {
        if(autoCD > 0.0f)
        {
            //autoCD -= Time.deltaTime;
            autoCD -= logicLen;
            if (autoCD <= 0.0f)
            {
                //Debug.Log("attack end1");
                animator.SetInteger("states", 0);
            }
            autoCD = Mathf.Max(autoCD, 0);
        }
    }
    void OneAttack()
    {
        //Debug.Log("one attack");
        autoCD = (float)(1.0 / unitManager.attackSpeed);
        animator.SetInteger("states", 2); // Set the animator to attack state; this will start the attack animation.
        animator.SetFloat("attackSpeedMultiplier", animationTime * unitManager.attackSpeed);
    }
    public void QuitAttack()
    {
        autoCD = 0.0f;
        animator.SetInteger("states", 0);
    }
}
