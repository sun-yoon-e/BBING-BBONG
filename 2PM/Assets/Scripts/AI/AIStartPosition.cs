using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStartPosition : MonoBehaviour
{
    RoadGenerator road;
    NavMeshAgent agent;

    public GameObject AIPrefab;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        GameObject AINavmeshAgent = new GameObject("AI Navmesh Agent");

        AINavmeshAgent.AddComponent<NavMeshAgent>();
        agent = AINavmeshAgent.GetComponent<NavMeshAgent>();
        
        transform.position = road.vertices[road.vertices.Length / 2 + 1 + road.xSize + 1];
        agent.transform.position = transform.position;

        Instantiate(AIPrefab, transform.position, Quaternion.identity);
        agent.SetDestination(road.vertices[road.vertices.Length / 2 + 1 + (road.xSize + 1) * 10]);
        agent.avoidancePriority = 0;
    }
}
