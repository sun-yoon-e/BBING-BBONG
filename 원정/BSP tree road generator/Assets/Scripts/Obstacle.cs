using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;
using UnityEngine.AI;

public class Obstacle : MonoBehaviour
{
    RoadGenerator road;
    public GameObject carPrefab;
    public Transform parent;

    

    GameObject[] car;

    void Start()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        car = new GameObject[100];

        GenerateCar();
    }

    private void Update()
    {
        foreach (var i in car)
        {
            if (i == false)
                continue;

            //i.transform.position += new Vector3(0, 0, 10 * Time.deltaTime);
            i.transform.rotation = Quaternion.Euler(0, 180, 0);

        }
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
                for (int j = road.xSize * road.zSize - 200; j < road.xSize * road.zSize; ++j)
                {
                    if (road.isObstaclePlace[j] == true)
                    {    
                        targetPosition = road.vertices[j];
                        break;
                    }
                }
                
                rand = Random.Range(0, 2);

                if (rand == 1)
                {
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
                    carPosition = new Vector3(road.vertices[i].x, road.vertices[i].y, road.vertices[i].z + 10);

                    car[carNum] = Instantiate(carPrefab, carPosition, Quaternion.Euler(0, 180, 0), parent);
                    car[carNum].AddComponent<NavMeshAgent>();

                    agent = car[carNum].GetComponent<NavMeshAgent>();
                    agent.SetDestination(targetPosition);

                    //rigidbody = GetComponent<Rigidbody>();

                    //pathFound = agent.CalculatePath(targetPosition, road.path);

                    //if (pathFound)
                    //{
                    //    agent.SetPath(road.path);
                    //}

                    ++carNum;
                }
            }
        }
    }
}