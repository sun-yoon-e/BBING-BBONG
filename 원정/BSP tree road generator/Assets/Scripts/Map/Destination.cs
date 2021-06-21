using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Destination : MonoBehaviour
{
    PlacementBuilding building;

    public GameObject destinationPrefab;
    public Sprite destinationSprite;
    public Transform parent;

    public int[] destination = null;
    public int destinationNum;
    private bool isReady = false;

    bool[] isDestination;

    public GameObject[] destinationPizzaObject;
    public GameObject[] destinationSpriteObject;
    public SpriteRenderer[] pizzaSpriteRenderer;

    public int DestroyDestination;

    private void Start()
    {
        building = GameObject.Find("BuildingGenerator").GetComponent<PlacementBuilding>();
        building.OnBuildingReady += OnBuildingReady;
        if (building.isBuildingReady)
        {
            OnBuildingReady(this, EventArgs.Empty);
        }
    }

    private void OnBuildingReady(object sender, EventArgs args)
    {
        if (isReady == false)
        {
            isReady = true;
            destination = new int[destinationNum];
            isDestination = new bool[building.buildingNum];
            DestroyDestination = 0;

            destinationPizzaObject = new GameObject[destinationNum];
            destinationSpriteObject = new GameObject[destinationNum];
            pizzaSpriteRenderer = new SpriteRenderer[destinationNum];
        }
        else
        {
            DrawDestination();
            ApplyDestinationToBuilding();
        }
    }

    private void Update()
    {
        if (destination == null) return;
       
        if (DestroyDestination == destinationNum)
        {
            DestroyDestination = 0;
            DrawDestination();
            ApplyDestinationToBuilding();
        }
    }

    public void DrawDestination()
    {
        for (int i = 0; i < destinationNum; ++i)
        {
            destination[i] = Random.Range(1, building.buildingNum);
            
            //중복체크
            for (int j = 0; j < destinationNum; ++j)
            {
                if (destination[i] == destination[j])
                {
                    destination[i] = Random.Range(1, building.buildingNum);
                }
            }

            isDestination[destination[i]] = true;
        }
    }

    void ApplyDestinationToBuilding()
    {
        Quaternion SpriteRotation = Quaternion.Euler(90, 0, 0);
        Vector3 ObjectScale = new Vector3(5, 5, 5);

        for (int i = 0; i < destinationNum; ++i)
        {
            Vector3 destinationPosition =
                new Vector3(building.buildingObject[destination[i]].transform.position.x,
                building.buildingObject[destination[i]].transform.position.y + 20,
                building.buildingObject[destination[i]].transform.position.z);

            destinationPizzaObject[i] = Instantiate(destinationPrefab, destinationPosition,
                Quaternion.Euler(0, 0, 0), parent);
            destinationPizzaObject[i].layer = 9;

            destinationSpriteObject[i] = new GameObject("DestinationSprite");
            destinationSpriteObject[i].transform.position = destinationPosition;
            destinationSpriteObject[i].transform.rotation = SpriteRotation;
            destinationSpriteObject[i].transform.localScale = ObjectScale;

            pizzaSpriteRenderer[i] = destinationSpriteObject[i].AddComponent<SpriteRenderer>();
            pizzaSpriteRenderer[i].sprite = destinationSprite;
            destinationSpriteObject[i].transform.SetParent(parent);

            destinationSpriteObject[i].layer = 8;
        }
    }
}
