using UnityEngine;
using UnityEditor.AI;
using UnityEngine.AI;

public class Obstacle : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform parent;

    RoadGenerator road;
    GameObject[] car;

    //float sightRange;

    public LayerMask wayPointLayer;

    void Start()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        car = new GameObject[100];

        //sightRange = 100f;

        GenerateCar();
    }

    private void Update()
    {
        //foreach (var i in car)
        //{
        //    if (i == false)
        //        continue;

        //    // if (destination이 비어있을 때, destination에 도달했을 때 호출)
        //    int rand = Random.Range(0, road.wayPointNum);
        //    NavMeshAgent agent = i.GetComponent<NavMeshAgent>();
        //    agent.SetDestination(road.wayPoint[rand].transform.position);
            
        //    //bool inSightRange = Physics.CheckSphere(i.transform.position, sightRange, wayPointLayer);
        //    //if (inSightRange == true) { }
        //}
    }

    void GenerateCar()
    {
        int carNum = 0;
        int rand;

        Vector3 carPosition;
        Vector3 targetPosition = new Vector3(0, 0, 0);

        NavMeshAgent agent;

        for (int i = 1; i < road.xSize; ++i)
        {
            if (road.isObstaclePlace[i] == true)
            {
                rand = Random.Range(0, 2);

                if (rand == 1)
                {
                    carPosition = new Vector3(road.vertices[i].x, road.vertices[i].y, road.vertices[i].z);

                    car[carNum] = Instantiate(carPrefab, carPosition, Quaternion.Euler(0, 180, 0), parent);

                    agent = car[carNum].GetComponent<NavMeshAgent>();

                    int randWayPoint = Random.Range(0, road.wayPointNum);
                    agent.SetDestination(road.wayPoint[randWayPoint].transform.position);
                    agent.updatePosition = false;
                    agent.updateRotation = false;

                    ++carNum;
                    /*
                    path = new NavMeshPath();
                    NavMeshAgent = GetComponent<NavMeshAgent>();
                    rigidbody = GetComponent<Rigidbody>();
    
                    bool pathFound = NavMeshAgent.CalculatePath(CurrentDestination, path);
               
                    if (pathFound)
                    {
                        NavMeshAgent.SetPath(path);
                    }
                     */

                    //rigidbody = GetComponent<Rigidbody>();

                    //pathFound = agent.CalculatePath(targetPosition, road.path);

                    //if (pathFound)
                    //{
                    //    agent.SetPath(road.path);
                    //}
                }
            }
        }
    }
}