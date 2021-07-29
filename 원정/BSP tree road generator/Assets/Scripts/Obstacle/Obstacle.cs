using UnityEngine;
using UnityEditor.AI;
using UnityEngine.AI;

public class Obstacle : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public Transform parent;

    RoadGenerator road;
    GameObject[] car;

    public LayerMask wayPointLayer;

    void Start()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        car = new GameObject[100];

        GenerateCar();
    }

    void GenerateCar()
    {
        int carNum = 0;
        int rand;

        Vector3 carPosition;

        for (int i = 1; i < road.xSize * road.zSize; ++i)
        {
            if (road.isWayPointPlace[i] == true)
            {
                rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    carPosition = road.vertices[i + road.xSize + 1];

                    var randomINdex = Random.Range(0, carPrefabs.Length);
                    car[carNum] = Instantiate(carPrefabs[randomINdex], carPosition, Quaternion.identity, parent);

                    car[carNum].GetComponent<NavMeshAgent>().avoidancePriority = 0;

                    ++carNum;
                }
            }
        }
    }
}