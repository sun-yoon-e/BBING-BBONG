using UnityEngine;
using UnityEngine.AI;

public class ObstacleGenerator : MonoBehaviour
{
    public GameObject[] carPrefabs;

    RoadGenerator road;
    GameObject[] car;

    public LayerMask wayPointLayer;

    public int carNum;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        car = new GameObject[road.wayPoint.Length];
        carNum = 0;

        GenerateCar(true);
    }

    public void GenerateCar(bool isGenerateManyCar)
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
                    carPosition = road.vertices[i + road.xSize + 1];

                    var randomINdex = Random.Range(0, carPrefabs.Length);
                    car[carNum] = Instantiate(carPrefabs[randomINdex], carPosition, Quaternion.identity, transform);

                    car[carNum].GetComponent<NavMeshAgent>().avoidancePriority = 0;

                    ++carNum;
                    if (!isGenerateManyCar)
                        break;
                }
            }
        }
    }
}