using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    RoadGenerator road;
    Rigidbody rb;

    Vector3 moveDirection;
    Vector3 direction;

    GameObject navObject;
    NavMeshAgent agent;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        rb = GetComponent<Rigidbody>();

        CreateNavMeshAgentObject();
    }

    private void Update()
    {
        moveDirection = Vector3.zero;
        direction = agent.transform.position - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
    }

    void FixedUpdate()
    {
        //rb.AddForce(moveDirection * 500000f);
    }

    void CreateNavMeshAgentObject()
    {
        navObject = new GameObject("AI NavMesh Agent");
        navObject.transform.position = transform.position;
        agent = navObject.AddComponent<NavMeshAgent>();

        //agent = navObject.GetComponent<NavMeshAgent>();

        for (int i = 1; i < road.vertices.Length; ++i)
        {
            if (road.isWayPointPlace[i] == true)
            {
                agent.SetDestination(road.vertices[i]);
            }
        }
        //agent.velocity = new Vector3(10f, 10f, 10f);
    }
}
