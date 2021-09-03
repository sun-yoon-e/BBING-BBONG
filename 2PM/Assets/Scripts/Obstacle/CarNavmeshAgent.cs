using UnityEngine;
using UnityEngine.AI;

public class CarNavmeshAgent: MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    NavMeshAgent agent;
    RoadGenerator road;
    ObstacleGenerator obstacle;

    int stopTime = 0;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();

        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        int carPlace = Random.Range(1, road.middleRoadNum);
        agent.SetDestination(road.passibleItemPlace[carPlace]);
        tag = "Car";
    }

    private void Update()
    {
        if (!gameTimer.instance.isStart)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
        
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            int carPlace = Random.Range(1, road.middleRoadNum);
            agent.SetDestination(road.passibleItemPlace[carPlace]);
        }

        if (agent.isStopped)
        {
            ++stopTime;
            if (stopTime > 200)
            {
                agent.isStopped = false;
                stopTime = 0;
            }
        }

        if (gameClient.isGameStarted && gameClient.client_host)
        {
            if (gameObject != null)
                obstacle.MoveCar(gameObject, transform.position, transform.rotation.eulerAngles);
        }
    }
}