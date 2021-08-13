using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    RoadGenerator road;
    public Rigidbody rb;

    public Vector3 direction;
    //public Quaternion rot;

    GameObject navObject;
    [HideInInspector] public NavMeshAgent agent;

    [HideInInspector] public float cal;

    public bool isLookAgent = false;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        rb = GetComponent<Rigidbody>();

        CreateNavMeshAgentObject();
    }

    private void Update()
    {
        direction = agent.transform.position - transform.position;

        if (direction.x > 8f || direction.x < -8f || direction.z > 8f || direction.z < -8f)
            agent.isStopped = true;
        else
            agent.isStopped = false;

        //rot = Quaternion.Euler(agent.transform.rotation.x - transform.rotation.x,
        //    agent.transform.rotation.y - transform.rotation.y,
        //    agent.transform.rotation.z - transform.rotation.z);

        if (isLookAgent)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
        else
            transform.rotation = agent.transform.rotation;

        //if (agent.remainingDistance <= agent.stoppingDistance)
        //{
        //    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0)
        //    {
        //        AIRBController rbController = GetComponent<AIRBController>();
        //        rbController.topGearTorque = 0;
        //        rbController.firstGearTorque = 0;
        //    }
        //}

        CalculateDirection();

        StartCoroutine("LookNavMeshAgent");
    }

    IEnumerator LookNavMeshAgent()
    {
        yield return new WaitForSeconds(1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
    }

    void CreateNavMeshAgentObject()
    {
        navObject = new GameObject("AI NavMesh Agent");
        navObject.transform.position = transform.position;
        
        agent = navObject.AddComponent<NavMeshAgent>();
        agent.speed = 20f;
        agent.radius = 2f;
        agent.avoidancePriority = 1;
        agent.angularSpeed = 60f;

        for (int i = 1; i < road.vertices.Length; ++i)
        {
            if (road.isWayPointPlace[i] == true)
            {
                agent.SetDestination(road.vertices[i]);
            }
        }
    }

    void CalculateDirection()
    {
        cal = Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
        //print(cal);
    }
}
