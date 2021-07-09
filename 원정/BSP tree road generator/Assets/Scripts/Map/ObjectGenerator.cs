using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    RoadGenerator road;
    //PlacementBuilding building;

    public GameObject[] objectPrefab;

    private void Awake()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        //building = GameObject.Find("BuildingGenerator").GetComponent<PlacementBuilding>();
    }

    private void Start()
    {
        GameObject mapObject;

        int prefab;
        int isObject;

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.buildingState[i] != 0 || road.isRoad[i] == true)
                continue;

            isObject = Random.Range(0, 4);
            if (isObject == 1)
            {
                prefab = Random.Range(0, objectPrefab.Length);
                mapObject = Instantiate(objectPrefab[prefab], road.vertices[i], Quaternion.identity);

                mapObject.transform.SetParent(transform);
            }
        }
    }
}
