using System.Collections.Generic;
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

    public int CarNum;
    public int num = 0;
    public GameObject[] CARS;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        //car = new GameObject[road.wayPoint.Length];
        Cars = new List<CarObject>();
        CarNum = 0;

        gameClient.OnMakeCar += OnMakeCar;
        gameClient.OnMoveCar += OnMoveCar;
        gameClient.OnDestroyCar += OnDestroyCar;

        road.OnRoadReady4 += CreateCar;
        if (road.isRoadReady)
        {
            CreateCar(this, System.EventArgs.Empty);
        }
    }

    private void Update()
    {
        if (gameClient.isGameStarted && gameClient.client_host)
        {
            //for (int i = 0; i < Cars.Count; i++)
            //{
            //    GameClient.Instance.MoveCar(i, Cars[i].Car.transform.position);
            //}
            for (int i = 0; i < CARS.Length; i++)
            {
                GameClient.Instance.MoveCar(i, CARS[i].transform.position);
            }
        }
    }

    void CreateCar(object sender, System.EventArgs args)
    {
        if (gameClient.client_host)
        {
            int rand;
            Vector3 carPosition;
            int carNum = 0;

            for (int i = 1; i < road.vertices.Length; ++i)
            {
                if (road.isWayPointPlace[i] == true)
                {
                    rand = Random.Range(0, 2);
                    if (rand == 1)
                    {
                        int carID = carNum++;
                        int carType = Random.Range(0, carPrefabs.Length);
                        carPosition = road.vertices[i + road.xSize + 1];

                        //GameClient.Instance.MakeCar(carID, (byte)carType, carPosition);
                        GameClient.Instance.CarInfo[num] = new MakeCarMessageEventArgs();
                        GameClient.Instance.CarInfo[num].ID = carID;
                        GameClient.Instance.CarInfo[num].CarType = (byte)carType;
                        GameClient.Instance.CarInfo[num].Position = carPosition;

                        //Debug.Log("OnRoadCar");
                    }
                }
            }
        }
    }

    public void OnMoveCar(object sender, MoveCarMessageEventArgs args)
    {
        //Cars[args.ID].Car.transform.position = args.Position;
        CARS[args.ID].transform.position = args.Position;
    }

    public void OnMakeCar(object sender, MakeCarMessageEventArgs args)
    {
        //CarObject car = new CarObject();
        //car.ID = args.ID;
        //car.Car = Instantiate(carPrefabs[args.CarType], args.Position, Quaternion.identity, transform);
        //car.Car.GetComponent<NavMeshAgent>().avoidancePriority = 0;

        //Cars.Add(car);

        //if (args.ID > CarNum)
        //    CarNum = args.ID;

        Debug.Log(args.ID + "Make");
        CARS[args.ID] = new GameObject();
        CARS[args.ID] = Instantiate(carPrefabs[args.CarType], args.Position, Quaternion.identity, transform);
        CARS[args.ID].GetComponent<NavMeshAgent>().avoidancePriority = 0;

        if (args.ID > CarNum)
            CarNum = args.ID;
    }

    public void OnDestroyCar(object sender, DestroyCarMessageEventArgs args)
    {
        //Debug.Log("onCar");
        //CarObject car = Cars.Find(p => p.Car == Cars[args.ID].Car);
        //if (car != null)
        //{
        //    Destroy(Cars[args.ID].Car);
        //    if (gameClient.client_host)
        //        GenerateCar();

        //    //Debug.Log("Destroy car");
        //}

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
                gameClient.DestroyCar(i);
        }
    }

    public void GenerateCar()
    {
        //Debug.Log("Generate Car");
        int rand;
        Vector3 carPosition;
        int carNum = CarNum;

        for (int i = 1; i < road.vertices.Length; ++i)
        {
            if (road.isWayPointPlace[i] == true)
            {
                rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    carNum++;
                    int carID = carNum;
                    int carType = Random.Range(0, carPrefabs.Length);
                    carPosition = road.vertices[i + road.xSize + 1];

                    GameClient.Instance.MakeCar(carID, (byte)carType, carPosition);
                    break;
                }
            }
        }
    }
}