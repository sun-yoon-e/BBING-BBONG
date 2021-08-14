using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    RoadGenerator road;
    public Rigidbody rb;

    public Vector3 direction;

    GameObject navObject;
    [HideInInspector] public NavMeshAgent agent;

    [HideInInspector] public float distance;

    public bool isLookAgent = false;
    public bool isArriveDestination = false;

    Vector3 stopPosition;
    bool isStopPosition;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        rb = GetComponent<Rigidbody>();

        CreateNavMeshAgentObject();
    }

    private void Update()
    {
        CalculateDirection();
        CheckIsSetDestination();

        if (isArriveDestination)
        {
            if (!isStopPosition)
            {
                stopPosition = transform.position;
                isStopPosition = true;
            }

            transform.position = stopPosition;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
        }
    }

    void CreateNavMeshAgentObject()
    {
        navObject = new GameObject("AI NavMesh Agent");
        navObject.transform.position = transform.position;
        
        agent = navObject.AddComponent<NavMeshAgent>();

        agent.speed = 20f;
        agent.radius = 0.1f;
        agent.acceleration = 50f;
        agent.avoidancePriority = 1;
        agent.tag = "AI";
        agent.autoBraking = true;

        SetAIDestination();
    }

    void SetAIDestination()
    {
        int destination = Random.Range(1, road.middleRoadNum);
        agent.SetDestination(road.passibleItemPlace[destination]);
    }

    void CheckIsSetDestination()
    {
        if( agent.transform.position == agent.destination)
        {
            isArriveDestination = true;
        }
    }

    void CalculateDirection()
    {
        direction = agent.transform.position - transform.position;
        distance = Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
        
        if (distance > 5)
            agent.isStopped = true;
        else
            agent.isStopped = false;
    }
}
