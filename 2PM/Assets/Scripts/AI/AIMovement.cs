using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using System;

public class AIMovement : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    RoadGenerator road;
    public Rigidbody rb;

    public Vector3 direction;

    GameObject navObject;
    [HideInInspector] public NavMeshAgent agent;

    [HideInInspector] public float distance;

    public bool isLookAgent = false;
    public bool isArriveDestination = false;

    Vector3 stopPosition;
    public bool isStopPosition;

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

        gameClient.OnAIPositionUpdated += AIPositionUpdated;

        if (isArriveDestination)
        {
            //if (!isStopPosition)
            //{
            //    stopPosition = transform.position;
            //    isStopPosition = true;
            //}
            //transform.position = stopPosition;

            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.07f);


        Vector3 rot = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        //gameClient.UpdatePositionAI(id, transform.position, rot);
    }

    public void AIPositionUpdated(object sender, AIPositionUpdateEventArgs args)
    {
        //args.AIID;
        //args.position;
        //args.rotation;
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

    public void SetAIDestination()
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
        
        if (distance > 7)
            agent.isStopped = true;
        else
            agent.isStopped = false;
    }
}
