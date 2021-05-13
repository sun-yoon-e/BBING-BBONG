using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementBuilding : MonoBehaviour
{
    public RoadGenerator road;
    public MeshGenerator map;

    public GameObject[] buildingPrefab;
    public GameObject buildingParent;

    float tempXSize;
    float tempZSize;

    private void Awake()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        map = GameObject.Find("MapGenerator").GetComponent<MeshGenerator>();
    }

    private void Start()
    {
        tempXSize = 0;
        tempZSize = 0;

        GameObject building;

        for (int i = 0; i < road.vertices.Length; ++i)
        {
            if (road.isBuildingPlace[i] == (int)buildingDirection.NOTBUILDINGPLACE)
                continue;

            int prefab = Random.Range(0, buildingPrefab.Length);
            Vector3 size = buildingPrefab[prefab].GetComponent<MeshRenderer>().bounds.size;

            tempXSize = size.x / 12;
            if (tempXSize < 1)
                tempXSize = 0.5f;
            else if (tempXSize >= 2)
                tempXSize = 1.0f;

            i += (int)tempXSize;

            if (((i+road.xSize + 1) < (road.xSize * road.zSize)) && 
                road.isBuildingPlace[i + road.xSize + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                tempZSize = size.x / 12;
                if (tempZSize < 1)
                    tempZSize = 0.5f;
                else if (tempZSize >= 2)
                    tempZSize = 1.0f;

                road.isBuildingPlace[i + (road.xSize + 1)] = (int)buildingDirection.NOTBUILDINGPLACE;
            }

            if (road.isBuildingPlace[i] == (int)buildingDirection.DOWN)
                building = Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.identity);
            else if (road.isBuildingPlace[i] == (int)buildingDirection.UP)
                building = Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 180, 0));
            else if (road.isBuildingPlace[i] == (int)buildingDirection.LEFT)
                building = Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, 90, 0));
            else if (road.isBuildingPlace[i] == (int)buildingDirection.RIGHT)
                building = Instantiate(buildingPrefab[prefab], road.vertices[i], Quaternion.Euler(0, -90, 0));
            else
                continue;

            makeNotBuildingPlace(i);

            building.transform.SetParent(buildingParent.transform);

            if (road.isBuildingPlace[i + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
            {
                tempXSize += size.x / 12;
                if (tempXSize < 1)
                    tempXSize = 1.0f;
                else if (tempXSize >= 2)
                    tempXSize = 2.0f;

                i += (int)tempXSize;
            }
            if (((i + road.xSize + 1) < (road.xSize * road.zSize)) &&
                road.isBuildingPlace[i + road.xSize + 1] != (int)buildingDirection.NOTBUILDINGPLACE)
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
            
            building.AddComponent<BoxCollider>();
            BoxCollider col = building.GetComponent<BoxCollider>();
            col.tag = "buildingBoxCollider";
        }

        map.UpdateMesh();
        road.RefreshRoadVertices();
        road.UpdateMesh();
    }

    void makeNotBuildingPlace(int place)
    {
        //road.isBuildingPlace[place] = 5;

        road.isBuildingPlace[place - 1] = 0;
        road.isBuildingPlace[place + 1] = 0;

        road.isBuildingPlace[place - road.xSize - 1] = 0;
        road.isBuildingPlace[place - road.xSize] = 0;
        road.isBuildingPlace[place - road.xSize - 2] = 0;

        road.isBuildingPlace[place + road.xSize] = 0;
        road.isBuildingPlace[place + road.xSize + 1] = 0;
        road.isBuildingPlace[place + road.xSize + 2] = 0;


        map.vertices[place - 1].y = map.vertices[place].y;
        map.vertices[place + 1].y = map.vertices[place].y;

        map.vertices[place - road.xSize - 1].y = map.vertices[place].y;
        map.vertices[place - road.xSize].y = map.vertices[place].y;
        map.vertices[place - road.xSize - 2].y = map.vertices[place].y;

        map.vertices[place + road.xSize].y = map.vertices[place].y;
        map.vertices[place + road.xSize + 1].y = map.vertices[place].y;
        map.vertices[place + road.xSize + 2].y = map.vertices[place].y;
    }


    enum buildingDirection
    {
        NOTBUILDINGPLACE,
        DOWN,
        UP,
        RIGHT,
        LEFT,
        PIZZABUILDING
    };
}
