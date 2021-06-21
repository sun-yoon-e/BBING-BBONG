using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlacementPizza : MonoBehaviour
{
    public GameObject pizzaBuildingPrefab;
    public Sprite pizzaBuildingSprite;

    public GameObject pizzaSpriteObject;
    public SpriteRenderer pizzaSpriteRenderer;

    RoadGenerator road;
    MeshGenerator map;

    private void Awake()
    {
        
    }

    private void Start()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        map = GameObject.Find("MapGenerator").GetComponent<MeshGenerator>();

        road.OnRoadReady2 += OnRoadReady;
        if (road.isRoadReady)
        {
            OnRoadReady(this, EventArgs.Empty);
        }
    }

    private void OnRoadReady(object sender, EventArgs args)
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

        // 건물 위치
        Vector3 pos = pizzaStore.transform.position;
        Vector3 buildingSize = pizzaStore.GetComponent<MeshRenderer>().bounds.size;
        Debug.Log("피자가게 위치 -> " + pos + ", " + buildingSize);

        pizzaStore.AddComponent<BoxCollider>();
        BoxCollider col = pizzaStore.GetComponent<BoxCollider>();
        col.tag = "PizzaStore";

        InitializeSprite(pizzaStore.transform.position);
    }

    void makeNotBuildingPlace(int place)
    {
        road.isBuildingPlace[place] = 5;

        if (place + 1 < road.xSize * road.zSize
             || place - 1 > 0)
        {
            road.isBuildingPlace[place - 1] = 0;
            road.isBuildingPlace[place + 1] = 0;
            map.vertices[place - 1].y = map.vertices[place].y;
            map.vertices[place + 1].y = map.vertices[place].y;
        }

        if (place - road.xSize - 2 > 0)
        {
            road.isBuildingPlace[place - road.xSize - 1] = 0;
            road.isBuildingPlace[place - road.xSize] = 0;
            road.isBuildingPlace[place - road.xSize - 2] = 0;
            map.vertices[place - road.xSize - 1].y = map.vertices[place].y;
            map.vertices[place - road.xSize].y = map.vertices[place].y;
            map.vertices[place - road.xSize - 2].y = map.vertices[place].y;
        }

        if (place + road.xSize + 2 < road.xSize * road.zSize)
        {
            road.isBuildingPlace[place + road.xSize] = 0;
            road.isBuildingPlace[place + road.xSize + 1] = 0;
            road.isBuildingPlace[place + road.xSize + 2] = 0;
            map.vertices[place + road.xSize].y = map.vertices[place].y;
            map.vertices[place + road.xSize + 1].y = map.vertices[place].y;
            map.vertices[place + road.xSize + 2].y = map.vertices[place].y;
        }
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
}