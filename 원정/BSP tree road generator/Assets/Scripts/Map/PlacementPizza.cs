using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPizza : MonoBehaviour
{
    public GameObject pizzaBuildingPrefab;
    public Sprite pizzaBuildingSprite;

    public GameObject pizzaSpriteObject;
    public SpriteRenderer pizzaSpriteRenderer;

    RoadGenerator road;
    MeshGenerator map;

    Quaternion rot;

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

        if (road.isRoad[(xSize * zSize) / 2 + (5 + xSize)] == false
            && road.isRoad[(xSize * zSize) / 2 + (5 + xSize)  + (road.xSize + 1) * 3 - 1] == false
             && road.isRoad[(xSize * zSize) / 2 + (5 + xSize) - (road.xSize + 1) * 3 - 1] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[xSize * zSize / 2 + 5 + xSize], Quaternion.Euler(0, 0, 0));
            makeNotBuildingPlace((xSize * zSize) / 2 + 5 + xSize);
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 3 + xSize] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 - 3 + xSize], Quaternion.Euler(0, 180, 0));
            makeNotBuildingPlace((xSize * zSize) / 2 - 3 + xSize);
        }
        else if (road.isRoad[(xSize * zSize) / 2 + 4] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 + 4], Quaternion.Euler(0, 0, 0));
            makeNotBuildingPlace((xSize * zSize) / 2 + 4);
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 4] == false)
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 - 4], Quaternion.Euler(0, 180, 0));
            makeNotBuildingPlace((xSize * zSize) / 2 - 4);
        }
        else
        {
            pizzaStore = Instantiate(pizzaBuildingPrefab, road.vertices[(xSize * zSize) / 2 + 6 + (xSize * 2)], Quaternion.Euler(0, 0, 0));
            makeNotBuildingPlace((xSize * zSize) / 2 + 6 + (xSize * 2));
        }

        pizzaStore.AddComponent<BoxCollider>();
        BoxCollider col = pizzaStore.GetComponent<BoxCollider>();
        col.tag = "PizzaStore";
        pizzaStore.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);

        InitializeSprite(pizzaStore.transform.position);
    }

    private void Update()
    {
        rot = GameObject.Find("Player").transform.rotation;
        rot = Quaternion.Euler(90, rot.eulerAngles.y, rot.eulerAngles.z);
        pizzaSpriteObject.transform.rotation = rot;
    }

    void makeNotBuildingPlace(int place)
    {
        road.buildingState[place + (road.xSize + 1)] = (int)buildingDirection.BUILDING;
        for (int i = 1; i < 4; ++i)
        {
            road.buildingState[place + (road.xSize + 1) * i - 1] = (int)buildingDirection.BUILDING;
            road.buildingState[place + (road.xSize + 1) * i + 1] = (int)buildingDirection.BUILDING;
        }

        road.buildingState[place - 1] = (int)buildingDirection.BUILDING;
        road.buildingState[place] = (int)buildingDirection.PIZZABUILDING;
        road.buildingState[place + 1] = (int)buildingDirection.BUILDING;

        road.buildingState[place - (road.xSize + 1)] = (int)buildingDirection.BUILDING;
        for (int i = 1; i < 4; ++i)
        {
            road.buildingState[place - (road.xSize + 1) * i - 1] = (int)buildingDirection.BUILDING;
            road.buildingState[place - (road.xSize + 1) * i + 1] = (int)buildingDirection.BUILDING;
        }

        map.vertices[place + (road.xSize + 1) * 2 - 1].y = map.vertices[place].y;
        map.vertices[place + (road.xSize + 1) * 2].y = map.vertices[place].y;
        map.vertices[place + (road.xSize + 1) * 2 + 1].y = map.vertices[place].y;

        map.vertices[place + (road.xSize + 1) - 1].y = map.vertices[place].y;
        map.vertices[place + (road.xSize + 1)].y = map.vertices[place].y;
        map.vertices[place + (road.xSize + 1) + 1].y = map.vertices[place].y;

        map.vertices[place - 1].y = map.vertices[place].y;
        map.vertices[place + 1].y = map.vertices[place].y;

        map.vertices[place - (road.xSize + 1) - 1].y = map.vertices[place].y;
        map.vertices[place - (road.xSize + 1)].y = map.vertices[place].y;
        map.vertices[place - (road.xSize + 1) + 1].y = map.vertices[place].y;

        map.vertices[place - (road.xSize + 1) * 2 - 1].y = map.vertices[place].y;
        map.vertices[place - (road.xSize + 1) * 2].y = map.vertices[place].y;
        map.vertices[place - (road.xSize + 1) * 2 + 1].y = map.vertices[place].y;
    }

    void InitializeSprite(Vector3 pos)
    {
        Quaternion SpriteRotation = Quaternion.Euler(90, 0, 0);
        Vector3 SpriteScale = new Vector3(25, 25, 25);

        Vector3 destinationPosition =
            new Vector3(pos.x, pos.y + 20, pos.z);

        pizzaSpriteObject = new GameObject("DestinationSprite");
        pizzaSpriteObject.transform.position = destinationPosition;
        pizzaSpriteObject.transform.rotation = SpriteRotation;
        pizzaSpriteObject.transform.localScale = SpriteScale;

        pizzaSpriteRenderer = pizzaSpriteObject.AddComponent<SpriteRenderer>();
        pizzaSpriteRenderer.sprite = pizzaBuildingSprite;

        pizzaSpriteObject.layer = 8;
    }

    enum buildingDirection
    {
        NOTBUILDINGPLACE,
        DOWN,
        UP,
        RIGHT,
        LEFT,
        PIZZABUILDING,
        BUILDING,
    };
}