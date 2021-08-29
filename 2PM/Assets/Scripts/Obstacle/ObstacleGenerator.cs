﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleGenerator : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    public GameObject[] carPrefabs;

    public class CarObject
    {
        public int ID;
        public GameObject Car;
    }
    public CarObject c;

    public List<CarObject> Cars;

    RoadGenerator road;
    //GameObject[] car;

    public LayerMask wayPointLayer;

    public int num = 0;
    public GameObject[] CARS;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        //car = new GameObject[road.wayPoint.Length];
        Cars = new List<CarObject>();
        CARS = new GameObject[50];

        gameClient.OnMakeCar += OnMakeCar;
        gameClient.OnMoveCar += OnMoveCar;
        gameClient.OnDestroyCar += OnDestroyCar;

        road.OnRoadReady4 += CreateCar;
        if (road.isRoadReady)
        {
            CreateCar(this, System.EventArgs.Empty);
        }
    }

    void CreateCar(object sender, System.EventArgs args)
    {
        if (gameClient.client_host)
        {
            int rand;
            Vector3 carPosition;

            for (int i = 1; i < road.vertices.Length; ++i)
            {
                if (road.isWayPointPlace[i] == true)
                {
                    rand = Random.Range(0, 2);
                    if (rand == 1)
                    {
                        int carType = Random.Range(0, carPrefabs.Length);
                        carPosition = road.vertices[i + road.xSize + 1];

                        //gameClient.MakeCar(carID, (byte)carType, carPosition);
                        gameClient.CarInfo[num] = new MakeCarMessageEventArgs();
                        gameClient.CarInfo[num].ID = num;
                        gameClient.CarInfo[num].CarType = (byte)carType;
                        gameClient.CarInfo[num].Position = carPosition;
                        gameClient.CarNum = num;
                        num++;

                        //Debug.Log("OnRoadCar");
                    }
                }
            }
        }
    }

    public void OnMakeCar(object sender, MakeCarMessageEventArgs args)
    {
        CARS[args.ID] = Instantiate(carPrefabs[args.CarType], args.Position, Quaternion.identity, transform);
        CARS[args.ID].GetComponent<NavMeshAgent>().avoidancePriority = 0;
        if(!gameClient.client_host)
        {
            Destroy(CARS[args.ID].GetComponent<NavMeshAgent>());
            Destroy(CARS[args.ID].GetComponent<CarNavmeshAgent>());
        }
    }
    
    public void OnMoveCar(object sender, MoveCarMessageEventArgs args)
    {
        if (CARS[args.ID] != null && !gameClient.client_host)
        {
            CARS[args.ID].gameObject.transform.position = args.Position;
            CARS[args.ID].gameObject.transform.rotation = Quaternion.Euler(args.Rotation);
        }
    }

    public void MoveCar(GameObject c, Vector3 pos, Vector3 rot)
    {
        //Debug.Log("MoveCar");
        rot.y += 180f;
        for (int i = 0; i < CARS.Length; ++i)
        {
            if (CARS[i].gameObject == c)
            {
                //Debug.Log("MoveCAR : " + i);
                gameClient.MoveCar(i, pos, rot);
            }
        }
    }

    public void OnDestroyCar(object sender, DestroyCarMessageEventArgs args)
    {
        if (CARS[args.ID] != null)
        {
            Destroy(CARS[args.ID]);
            if (gameClient.client_host)
                GenerateCar();
        }
    }

    public void DestroyCar(GameObject c)
    {
        for (int i = 0; i < CARS.Length; ++i)
        {
            if (CARS[i].gameObject == c)
            {
                gameClient.DestroyCar(i);
                //Debug.Log("DestoryCAR : " + i);
            }
        }
    }

    public void GenerateCar()
    {
        int rand;
        Vector3 carPosition;

        for (int i = 1; i < road.vertices.Length; ++i)
        {
            if (road.isWayPointPlace[i] == true)
            {
                rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    int carID = num;
                    int carType = Random.Range(0, carPrefabs.Length);
                    carPosition = road.vertices[i + road.xSize + 1];
                    gameClient.MakeCar(carID, (byte)carType, carPosition);
                    num++;
                    break;
                }
            }
        }
    }
}