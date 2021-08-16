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
        building = GameObject.Find("Building Generator").GetComponent<PlacementBuilding>();
        building.OnBuildingReady2 += CreateObjectPrefab;
        if (building.isBuildingReady)
        {
            CreateObjectPrefab(this, EventArgs.Empty);
        }
    }

    private void Start()
    {

    }

    private void CreateObjectPrefab(object sender, EventArgs args)
    {
        int prefab = 0;
        int isObject;

        for (int i = 0; i < road.vertices.Length; i += 3)
        {
            if ((road.isObjectPlace[i] == false && road.buildingState[i] != 0) || road.isRoad[i] == true)
                continue;

            isObject = Random.Range(0, 4);
            if (isObject == 1)
            {
                prefab = Random.Range(0, objectPrefab.Length);
                GameClient.Instance.MakeTree((byte)prefab, road.vertices[i]);

                //Instantiate(objectPrefab[prefab], road.vertices[i], Quaternion.identity, transform);
                //prefab++;
                //if (prefab >= objectPrefab.Length)
                //    prefab = 0;
            }
        }
    }

    public void OnMakeTree(object sender, MakeTreeMessageEventArgs args)
    {
        Instantiate(objectPrefab[args.Type], args.Position, Quaternion.identity, transform);
    }
}