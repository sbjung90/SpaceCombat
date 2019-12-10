using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    public List<Transform> wayPoints;
    public int nextIndex;

    private NavMeshAgent agent;

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;

    private float damping = 1.0f;
    private Transform enemyTr;

    private bool patrolling;
    public bool Patrolling
    {
        get { return patrolling; }
        set
        {
            patrolling = value;
            if (patrolling)
            {
                agent.speed = patrolSpeed;
                MoveWayPoint();
                damping = 1.0f;
            }
        }
    }

    private Vector3 traceTarget;
    public Vector3 TraceTarget
    {
        get { return traceTarget; }
        set
        {
            traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7.0f;

            if (!agent.isPathStale)
            {
                agent.destination = value;
                agent.isStopped = false;
            }
        }
    }

    public float Speed
    {
        get { return agent.velocity.magnitude; }
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyTr = GetComponent<Transform>();

        agent.autoBraking = false;
        agent.updateRotation = false;

        agent.speed = patrolSpeed;

        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);

            wayPoints.RemoveAt(0);
            nextIndex = Random.Range(0, wayPoints.Count);
        }

        //MoveWayPoint();
        patrolling = true;
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        patrolling = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (agent.isStopped == false)
        {
            if (agent.desiredVelocity != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
                enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot,
                    Time.deltaTime * damping);
            }
        }

        if (!patrolling) return;

        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f 
            && agent.remainingDistance <= 0.5f)
        {
            //nextIndex = ++nextIndex % wayPoints.Count;
            nextIndex = Random.Range(0, wayPoints.Count);

            MoveWayPoint();
        }
    }

    void MoveWayPoint()
    {
        if (agent.isPathStale) return;

        agent.destination = wayPoints[nextIndex].position;
        agent.isStopped = false;
    }
}
