using System;
using UnityEngine;

public class PlacementPizzaStore : MonoBehaviour
{
    public GameObject pizzaStorePrefab;
    public Sprite pizzaStoreSprite;

    public GameObject pizzaStoreSpriteObject;
    public SpriteRenderer pizzaStoreSpriteRenderer;

    RoadGenerator road;
    MeshGenerator map;
    PlacementBuilding building;

    Quaternion rot;

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();
        map = GameObject.Find("Terrain Generator").GetComponent<MeshGenerator>();
        building = GameObject.Find("Building Generator").GetComponent<PlacementBuilding>();

        road.OnRoadReady += OnRoadReady;
        if (road.isRoadReady)
        {
            OnRoadReady(this, EventArgs.Empty);
        }
    }

    private void Start()
    {
    }
    
    private void OnRoadReady(object sender, EventArgs args)
    {
        int xSize = road.xSize;
        int zSize = road.zSize;
        GameObject pizzaStore = new GameObject();

        if (road.isRoad[(xSize * zSize) / 2 + (5 + xSize)] == false
            && road.isRoad[(xSize * zSize) / 2 + (5 + xSize) + (road.xSize + 1) * 3 - 1] == false
            && road.isRoad[(xSize * zSize) / 2 + (5 + xSize) - (road.xSize + 1) * 3 - 1] == false)
        {
            pizzaStore = Instantiate(pizzaStorePrefab, road.vertices[xSize * zSize / 2 + 5 + xSize], Quaternion.Euler(0, 0, 0));
            makeBuildingPlace((xSize * zSize) / 2 + 5 + xSize);
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 3 + xSize] == false)
        {
            pizzaStore = Instantiate(pizzaStorePrefab, road.vertices[(xSize * zSize) / 2 - 3 + xSize], Quaternion.Euler(0, 180, 0));
            makeBuildingPlace((xSize * zSize) / 2 - 3 + xSize);
        }
        else if (road.isRoad[(xSize * zSize) / 2 + 4] == false)
        {
            pizzaStore = Instantiate(pizzaStorePrefab, road.vertices[(xSize * zSize) / 2 + 4], Quaternion.Euler(0, 0, 0));
            makeBuildingPlace((xSize * zSize) / 2 + 4);
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 4] == false)
        {
            pizzaStore = Instantiate(pizzaStorePrefab, road.vertices[(xSize * zSize) / 2 - 4], Quaternion.Euler(0, 180, 0));
            makeBuildingPlace((xSize * zSize) / 2 - 4);
        }
        else
        {
            pizzaStore = Instantiate(pizzaStorePrefab, road.vertices[(xSize * zSize) / 2 + 6 + (xSize * 2)], Quaternion.Euler(0, 0, 0));
            makeBuildingPlace((xSize * zSize) / 2 + 6 + (xSize * 2));
        }

        pizzaStore.transform.localScale = new Vector3(building.buildingScale, building.buildingScale, building.buildingScale);
        pizzaStore.tag = "PizzaStore";

        InitializeSprite(pizzaStore.transform.position);
    }

    private void Update()
    {
        rot = GameObject.Find("Player").transform.rotation;
        rot = Quaternion.Euler(90, rot.eulerAngles.y, rot.eulerAngles.z);
        pizzaStoreSpriteObject.transform.rotation = rot;
    }

    void makeBuildingPlace(int place)
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

        pizzaStoreSpriteObject = new GameObject("DestinationSprite");
        pizzaStoreSpriteObject.transform.position = destinationPosition;
        pizzaStoreSpriteObject.transform.rotation = SpriteRotation;
        pizzaStoreSpriteObject.transform.localScale = SpriteScale;

        pizzaStoreSpriteRenderer = pizzaStoreSpriteObject.AddComponent<SpriteRenderer>();
        pizzaStoreSpriteRenderer.sprite = pizzaStoreSprite;

        pizzaStoreSpriteObject.layer = 8;
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