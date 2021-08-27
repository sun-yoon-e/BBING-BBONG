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
        //if (gameClient.client_host)
        //{
        //    for (int i = 0; i < Cars.Count; i++)
        //    {
        //        GameClient.Instance.MoveCar(i, Cars[i].Car.transform.position);
        //    }
        //}
    }

    void CreateCar(object sender, System.EventArgs args)
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

                    GameClient.Instance.MakeCar(carID, (byte)carType, carPosition);
                    //Debug.Log("OnRoadCar");
                }
            }
        }
        //GenerateCar();
    }

    public void OnMoveCar(object sender, MoveCarMessageEventArgs args)
    {
        for (int i = 0; i < Cars.Count; i++)
        {
            Cars[i].Car.transform.position = args.Position;
        }
    }

    public void OnMakeCar(object sender, MakeCarMessageEventArgs args)
    {
        CarObject car = new CarObject();
        car.ID = args.ID;
        car.Car = Instantiate(carPrefabs[args.CarType], args.Position, Quaternion.identity, transform);
        car.Car.GetComponent<NavMeshAgent>().avoidancePriority = 0;

        Cars.Add(car);

        if (args.ID > CarNum)
            CarNum = args.ID;

        //Debug.Log("MakeCar");
    }

    public void OnDestroyCar(object sender, DestroyCarMessageEventArgs args)
    {
        //Debug.Log("onCar");
        CarObject car = Cars.Find(p => p.Car == Cars[args.ID].Car);
        if (car != null)
        {
            Destroy(Cars[args.ID].Car);
            GenerateCar();

            //Debug.Log("Destroy car");
        }
    }

    public void DestroyCar()
    {
        for (int i = 0; i < Cars.Count; i++)
        {
            CarObject car = Cars.Find(p => p.ID == Cars[i].ID);
            GameClient.Instance.DestroyCar(car.ID);
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