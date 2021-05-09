using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementBuilding : MonoBehaviour
{
    public GameObject[] buildingPrefab;

    private void Start()
    {
        RoadGenerator road = GetComponent<RoadGenerator>();

        int num = 0;

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.isBuildingPlace[i] == (int)buildingDirection.NOTBUILDINGPLACE)
                continue;

            int prefab = Random.Range(0, buildingPrefab.Length);

            Vector3 size = buildingPrefab[prefab].GetComponent<MeshRenderer>().bounds.size;

            float tempXSize = 0;
            float tempZSize = 0;

            tempXSize = size.x / 12;
            if (tempXSize < 1)
                tempXSize = 0.5f;
            else if (tempXSize >= 2)
                tempXSize = 1.0f;

            i += (int)tempXSize;

            if (road.isBuildingPlace[i + road.xSize + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                tempZSize = size.x / 12;
                if (tempZSize < 1)
                    tempZSize = 0.5f;
                else if (tempZSize >= 2)
                    tempZSize = 1.0f;

                for (int j = 0; j < (int)tempZSize; ++j)
                {
                    road.isBuildingPlace[i + (road.xSize + 1) * j] = (int)buildingDirection.NOTBUILDINGPLACE;
                }
            }

            ++num;
            if (road.isBuildingPlace[i] == (int)buildingDirection.DOWN)
                Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.identity);
            else if (road.isBuildingPlace[i] == (int)buildingDirection.UP)
                Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 180, 0));
            else if (road.isBuildingPlace[i] == (int)buildingDirection.LEFT)
                Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 90, 0));
            else if (road.isBuildingPlace[i] == (int)buildingDirection.RIGHT)
                Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, -90, 0));

            if (road.isBuildingPlace[i + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                tempXSize += size.x / 12;
                if (tempXSize < 1)
                    tempXSize = 1.0f;
                else if (tempXSize >= 2)
                    tempXSize = 2.0f;

                i += (int)tempXSize;
            }

            if (road.isBuildingPlace[i + road.xSize + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                tempZSize = size.x / 12;
                if (tempZSize < 1)
                    tempZSize = 0.5f;
                else if (tempZSize >= 2)
                    tempZSize = 1.0f;

                for (int j = 0; j < (int)tempZSize; ++j)
                {
                    road.isBuildingPlace[i + road.xSize + 1] = (int)buildingDirection.NOTBUILDINGPLACE;
                }
            }
        }
        Debug.Log(num);
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
