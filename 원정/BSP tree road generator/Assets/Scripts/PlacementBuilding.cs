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
            if (road.isBuildingPlace[i] == false)
                continue;

            int prefab = Random.Range(1, buildingPrefab.Length);

            Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.identity);

            Vector3 size = buildingPrefab[prefab].GetComponent<MeshRenderer>().bounds.size;

            if (road.isBuildingPlace[i + 1])
            {
                i += ((int)size.x - 1) / 10;
            }

            if (road.isBuildingPlace[i + road.xSize + 1])
            {
                for (int j = 0; j < (int)size.x / 5; ++j)
                    if (i + (road.xSize + 1) * j < road.xSize * road.zSize)
                        road.isBuildingPlace[i + (road.xSize + 1) * j] = false;
            }
        }
    }
}
