﻿using UnityEngine;
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