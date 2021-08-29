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
    public bool isStopPosition;

    CreateAIID ID;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        rb = GetComponent<Rigidbody>();
        ID = transform.Find("AIID").GetComponent<CreateAIID>();

        CreateNavMeshAgentObject();
    }

    bool isStop = false;
    float stopStartTime = 0f;
    private void Update()
    {
        CalculateDirection();
        CheckIsSetDestination();


        if (isArriveDestination)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.07f);

        CheckAIStop();

        if (GameClient.Instance.client_host)
        {
            GameClient.Instance.UpdatePositionAI(ID.idNum, transform.position, transform.rotation.eulerAngles);
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

    public void SetAIDestination()
    {
        int destination = Random.Range(1, road.middleRoadNum);
        agent.SetDestination(road.passibleItemPlace[destination]);
    }

    void CheckIsSetDestination()
    {
        if (agent.transform.position == agent.destination)
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

    void CheckAIStop()
    {
        if (agent.isStopped == true)
        {
            if (!isStop)
            {
                stopStartTime = Time.time;
                isStop = true;
            }

            if (Time.time - stopStartTime > 15f)
            {
                gameObject.transform.position = road.vertices[road.vertices.Length / 2 + 1];
                agent.transform.position = road.vertices[road.vertices.Length / 2 + 1];
                stopStartTime = 0f;

                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        else
        {
            stopStartTime = 0f;
            isStop = false;
        }
    }
}