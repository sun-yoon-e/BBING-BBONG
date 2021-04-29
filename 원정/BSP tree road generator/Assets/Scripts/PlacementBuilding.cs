using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementBuilding : MonoBehaviour
{
    public GameObject[] buildingPrefab;

    private void Start()
    {
        RoadGenerator road = GetComponent<RoadGenerator>();

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.isBuildingPlace[i] == (int)buildingDirection.NOTBUILDINGPLACE)
                continue;

            int prefab = Random.Range(1, buildingPrefab.Length);

            if(road.isBuildingPlace[i] == (int)buildingDirection.DOWN)
            {
                Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.identity);
            }
            else if(road.isBuildingPlace[i] == (int)buildingDirection.UP)
            {
                Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 180, 0));
            }
            else if (road.isBuildingPlace[i] == (int)buildingDirection.LEFT)
            {
                Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 90, 0));
            }
            else if (road.isBuildingPlace[i] == (int)buildingDirection.RIGHT)
            {
                Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, -90, 0));
            }


            Vector3 size = buildingPrefab[prefab].GetComponent<MeshRenderer>().bounds.size;

            if (road.isBuildingPlace[i + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                i += ((int)size.x - 1) / 10;
            }
            if (road.isBuildingPlace[i + road.xSize + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                for (int j = 0; j < (int)size.x / 5; ++j)
                    if (i + (road.xSize + 1) * j < road.xSize * road.zSize)
                        road.isBuildingPlace[i + (road.xSize + 1) * j] = (int)buildingDirection.NOTBUILDINGPLACE;
            }
        }
    }

    enum buildingDirection
    {
        NOTBUILDINGPLACE,
        DOWN,
        UP,
        RIGHT,
        LEFT
    };
}
