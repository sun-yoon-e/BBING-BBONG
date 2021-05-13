using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPizza : MonoBehaviour
{
    public GameObject pizzaBuildingPrefab;
    public RoadGenerator road;

    private void Awake()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
    }

    private void Start()
    {
        int xSize = road.xSize;
        int zSize = road.zSize;

        if (road.isRoad[(xSize * zSize) / 2 + 2 + xSize] == false)
        {
            Instantiate(pizzaBuildingPrefab, road.vertices[xSize * zSize / 2 + 2 + xSize], Quaternion.Euler(0, 0, 0));
            road.isBuildingPlace[(xSize * zSize) / 2 + 2 + xSize] = 5;
            road.isBuildingPlace[(xSize * zSize) / 2 + 1] = 0;
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 1 + xSize] == false)
        {
            Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 - 1 + xSize], Quaternion.Euler(0, 180, 0));
            road.isBuildingPlace[(xSize * zSize) / 2 - 1 + xSize] = 5;
            road.isBuildingPlace[(xSize * zSize) / 2 - 2] = 0;
        }
        else if (road.isRoad[(xSize * zSize) / 2 + 1] == false)
        {
            Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 + 1], Quaternion.Euler(0, 0, 0));
            road.isBuildingPlace[(xSize * zSize) / 2 + 1] = 5;
            road.isBuildingPlace[(xSize * zSize) / 2 - xSize] = 0;
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 2] == false)
        {
            Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 - 2], Quaternion.Euler(0, 180, 0));
            road.isBuildingPlace[(xSize * zSize) / 2 - 2] = 5;
            road.isBuildingPlace[(xSize * zSize) / 2 - 3 - xSize] = 0;
        }
    }
}
