using UnityEngine;
using UnityEngine.AI;

public class CarNavmeshAgent : MonoBehaviour
{
    NavMeshAgent agent;
    RoadGenerator road;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        int randWayPoint = Random.Range(0, road.wayPointNum);
        agent.SetDestination(road.wayPoint[randWayPoint].transform.position);
        tag = "Car";
    }

    private void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            int randWayPoint = Random.Range(0, road.wayPointNum);
            agent.SetDestination(road.wayPoint[randWayPoint].transform.position);
        }
    }
}
