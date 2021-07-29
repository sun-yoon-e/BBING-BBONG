using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    NavMeshAgent agent;
    RoadGenerator road;

    private void Awake()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        int randWayPoint = Random.Range(0, road.wayPointNum);
        agent.SetDestination(road.wayPoint[randWayPoint].transform.position);
        //agent.avoidancePriority = 1;
    }

    private void Update()
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            int randWayPoint = Random.Range(0, road.wayPointNum);
            agent.SetDestination(road.wayPoint[randWayPoint].transform.position);
        }
    }
}
