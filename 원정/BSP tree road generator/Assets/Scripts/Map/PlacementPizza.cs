using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPizza : MonoBehaviour
{
    public GameObject pizzaBuildingPrefab;
    RoadGenerator road;
    MeshGenerator map;

    private void Awake()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        map = GameObject.Find("MapGenerator").GetComponent<MeshGenerator>();
    }

    private void Start()
    {
        int xSize = road.xSize;
        int zSize = road.zSize;
        GameObject pizzaStore = new GameObject();

        if (road.isRoad[(xSize * zSize) / 2 + 2 + xSize] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[xSize * zSize / 2 + 2 + xSize], Quaternion.Euler(0, 0, 0));
         
            makeNotBuildingPlace((xSize * zSize) / 2 + 2 + xSize);
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 1 + xSize] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 - 1 + xSize], Quaternion.Euler(0, 180, 0));
            
            makeNotBuildingPlace((xSize * zSize) / 2 - 1 + xSize);
        }
        else if (road.isRoad[(xSize * zSize) / 2 + 1] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 + 1], Quaternion.Euler(0, 0, 0));

            makeNotBuildingPlace((xSize * zSize) / 2 + 1);
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 2] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 - 2], Quaternion.Euler(0, 180, 0));

            makeNotBuildingPlace((xSize * zSize) / 2 - 2);
        }
        else //if (road.isRoad[(xSize * zSize) / 2 + 3 + (xSize * 2)] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 + 3 + (xSize * 2)], Quaternion.Euler(0, 0, 0));
            makeNotBuildingPlace((xSize * zSize) / 2 + 3 + (xSize * 2));
        }

        pizzaStore.AddComponent<BoxCollider>();
        BoxCollider col = pizzaStore.GetComponent<BoxCollider>();
        col.tag = "PizzaStore";

        //map.UpdateMesh();
    }
    
    void makeNotBuildingPlace(int place)
    {
        road.isBuildingPlace[place] = 5;

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
    
    
    

}
