using System;
using UnityEngine;

public class PlacementPizzaStore : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    public GameObject pizzaStorePrefab;
    public Sprite pizzaStoreSprite;

    public GameObject pizzaStoreSpriteObject;
    public SpriteRenderer pizzaStoreSpriteRenderer;

    RoadGenerator road;
    MeshGenerator map;
    PlacementBuilding building;

    public GameObject parent;

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
        gameClient.OnMakePizzaStore += OnMakePizzaStore;
    }

    private void OnRoadReady(object sender, EventArgs args)
    {
        int xSize = road.xSize;
        int zSize = road.zSize;

        Vector3 storePosition;
        Quaternion storeRotation;

        if (road.isRoad[(xSize * zSize) / 2 + (5 + xSize)] == false
            && road.isRoad[(xSize * zSize) / 2 + (5 + xSize) + (road.xSize + 1) * 3 - 1] == false
            && road.isRoad[(xSize * zSize) / 2 + (5 + xSize) - (road.xSize + 1) * 3 - 1] == false)
        {
            storePosition = road.vertices[(xSize * zSize) / 2 + 5 + xSize];
            storeRotation = Quaternion.Euler(0, 0, 0);

            makeBuildingPlace((xSize * zSize) / 2 + 5 + xSize);
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 3 + xSize] == false)
        {
            storePosition = road.vertices[(xSize * zSize) / 2 - 3 + xSize];
            storeRotation = Quaternion.Euler(0, 180, 0);

            makeBuildingPlace((xSize * zSize) / 2 - 3 + xSize);
        }
        else if (road.isRoad[(xSize * zSize) / 2 + 4] == false)
        {
            storePosition = road.vertices[(xSize * zSize) / 2 + 4];
            storeRotation = Quaternion.Euler(0, 0, 0);

            makeBuildingPlace((xSize * zSize) / 2 + 4);
        }
        else if (road.isRoad[(xSize * zSize) / 2 - 4] == false)
        {
            storePosition = road.vertices[(xSize * zSize) / 2 - 4];
            storeRotation = Quaternion.Euler(0, 180, 0);

            makeBuildingPlace((xSize * zSize) / 2 - 4);
        }
        else
        {
            storePosition = road.vertices[(xSize * zSize) / 2 + 6 + (xSize * 2)];
            storeRotation = Quaternion.Euler(0, 0, 0);

            makeBuildingPlace((xSize * zSize) / 2 + 6 + (xSize * 2));
        }

        if (gameClient.client_host)
        {
            gameClient.StoreInfo = new MakePizzaStoreMessageEventArgs();
            gameClient.StoreInfo.Position = storePosition;
            gameClient.StoreInfo.Rotation = storeRotation.eulerAngles;
        }
    }

    void makeBuildingPlace(int place)
    {
        for (int i = 1; i < 4; ++i)
        {
            road.buildingState[place + (road.xSize + 1) * i - 2] = (int)buildingDirection.BUILDING;
            road.buildingState[place + (road.xSize + 1) * i - 1] = (int)buildingDirection.BUILDING;
            road.buildingState[place + (road.xSize + 1) * i] = (int)buildingDirection.BUILDING;
            road.buildingState[place + (road.xSize + 1) * i + 1] = (int)buildingDirection.BUILDING;
            road.buildingState[place + (road.xSize + 1) * i + 2] = (int)buildingDirection.BUILDING;
        }

        road.buildingState[place - 2] = (int)buildingDirection.BUILDING;
        road.buildingState[place - 1] = (int)buildingDirection.BUILDING;
        road.buildingState[place] = (int)buildingDirection.PIZZABUILDING;
        road.buildingState[place + 1] = (int)buildingDirection.BUILDING;
        road.buildingState[place + 2] = (int)buildingDirection.BUILDING;

        for (int i = 1; i < 4; ++i)
        {
            road.buildingState[place - (road.xSize + 1) * i - 2] = (int)buildingDirection.BUILDING;
            road.buildingState[place - (road.xSize + 1) * i - 1] = (int)buildingDirection.BUILDING;
            road.buildingState[place - (road.xSize + 1) * i] = (int)buildingDirection.BUILDING;
            road.buildingState[place - (road.xSize + 1) * i + 1] = (int)buildingDirection.BUILDING;
            road.buildingState[place - (road.xSize + 1) * i + 2] = (int)buildingDirection.BUILDING;
        }

        map.vertices[place + (road.xSize + 1) - 2].y = map.vertices[place].y;
        map.vertices[place + (road.xSize + 1) - 1].y = map.vertices[place].y;
        map.vertices[place + (road.xSize + 1)].y = map.vertices[place].y;
        map.vertices[place + (road.xSize + 1) + 1].y = map.vertices[place].y;
        map.vertices[place + (road.xSize + 1) + 2].y = map.vertices[place].y;

        map.vertices[place - 2].y = map.vertices[place].y;
        map.vertices[place - 1].y = map.vertices[place].y;
        map.vertices[place + 1].y = map.vertices[place].y;
        map.vertices[place + 2].y = map.vertices[place].y;

        map.vertices[place - (road.xSize + 1) - 2].y = map.vertices[place].y;
        map.vertices[place - (road.xSize + 1) - 1].y = map.vertices[place].y;
        map.vertices[place - (road.xSize + 1)].y = map.vertices[place].y;
        map.vertices[place - (road.xSize + 1) + 1].y = map.vertices[place].y;
        map.vertices[place - (road.xSize + 1) + 2].y = map.vertices[place].y;
    }

    void InitializeSprite(Vector3 pos)
    {
        Quaternion SpriteRotation = Quaternion.Euler(90, 0, 0);
        Vector3 SpriteScale = new Vector3(25, 25, 25);

        Vector3 pizzaStorePosition =
            new Vector3(pos.x, pos.y + 20, pos.z);

        pizzaStoreSpriteObject = new GameObject("PizzaSprite");
        pizzaStoreSpriteObject.transform.position = pizzaStorePosition;
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

    public void OnMakePizzaStore(object sender, MakePizzaStoreMessageEventArgs args)
    {
        if (!gameClient.isRenderPizzaStore)
        {
            GameObject pizzaStore = new GameObject();
            pizzaStore = Instantiate(pizzaStorePrefab, args.Position, Quaternion.Euler(args.Rotation));

            pizzaStore.transform.localScale = new Vector3(building.buildingScale, building.buildingScale, building.buildingScale);
            pizzaStore.tag = "PizzaStore";
            pizzaStore.transform.SetParent(parent.transform);

            InitializeSprite(pizzaStore.transform.position);

            gameClient.isRenderPizzaStore = true;
        }
    }
}