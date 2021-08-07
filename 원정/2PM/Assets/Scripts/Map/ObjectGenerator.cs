using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectGenerator : MonoBehaviour
{
    PlacementBuilding building;
    RoadGenerator road;
    public GameObject[] objectPrefab;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
    }

    private void Start()
    {
        building = GameObject.Find("BuildingGenerator").GetComponent<PlacementBuilding>();
        building.OnBuildingReady2 += CreateObjectPrefab;
        if (building.isBuildingReady)
        {
            CreateObjectPrefab(this, EventArgs.Empty);
        }
    }
    
    private void CreateObjectPrefab(object sender, EventArgs args)
    {
        int prefab;
        int isObject;

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if ((road.isObjectPlace[i] == false && road.buildingState[i] != 0) || road.isRoad[i] == true)
                continue;

            isObject = Random.Range(0, 4);
            if (isObject == 1)
            {
                prefab = Random.Range(0, objectPrefab.Length);
                Instantiate(objectPrefab[prefab], road.vertices[i], Quaternion.identity, transform);
            }
        }
    }
}