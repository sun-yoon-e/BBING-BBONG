using UnityEngine;
using UnityEngine.AI;

public class CarNavmeshAgent: MonoBehaviour
{
    ObstacleGenerator obstacle;
    NavMeshAgent agent;
    RoadGenerator road;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
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

        // 렉 좀 심하고 제자리에서 뱅글뱅글 돌아서 일단 지움
        //for (int i = 0; i < obstacle.Cars.Count; i++)
        //{
        //    GameClient.Instance.MoveCar(i, obstacle.Cars[i].Car.transform.position);
        //}

    }
}
