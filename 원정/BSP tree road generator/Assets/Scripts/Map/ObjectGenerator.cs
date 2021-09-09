using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectGenerator : MonoBehaviour
{
    PlacementBuilding building;
    public RoadGenerator road;
    public GameObject[] objectPrefab;

    private void Awake()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
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
        Debug.Log("CreateObjectPrefab()");
        GameObject mapObject;

        int prefab;

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.isBuildingPlace[i] != 0 || road.isRoad[i] == true)
                continue;

            prefab = Random.Range(0, objectPrefab.Length);
            mapObject = Instantiate(objectPrefab[prefab], road.vertices[i], Quaternion.identity) as GameObject;

            mapObject.transform.SetParent(transform);
        }
    }
}
