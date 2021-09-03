using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Destination : MonoBehaviour
{
    public static Destination instance;
    PlacementBuilding building;

    public GameObject destinationPrefab;
    public Sprite destinationSprite;
    public Transform parent;

    public int[] destination;
    public int destinationNum;

    bool[] isDestination;

    public GameObject[] destinationObject;
    public GameObject[] destinationSpriteObject;
    public SpriteRenderer[] pizzaSpriteRenderer;

    public int DestroyDestination;

    Quaternion rot;
    
    private bool isReady = false;

    private void Start()
    {
        instance = this;
        building = GameObject.Find("Building Generator").GetComponent<PlacementBuilding>();
        
        Invoke("CreateDestination", 1f);
    }

    void CreateDestination()
    {
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

            destinationObject = new GameObject[destinationNum];
            destinationSpriteObject = new GameObject[destinationNum];
            pizzaSpriteRenderer = new SpriteRenderer[destinationNum];

            DrawDestination();
            ApplyDestinationToBuilding();
        }
    }

    private void Update()
    {
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

            destinationObject[i] = Instantiate(destinationPrefab, destinationPosition,
                Quaternion.Euler(0, 0, 0), parent);
            destinationObject[i].layer = 9;
            //destinationObject[i].transform.parent = SelectionOutlineController.instance.target.transform;
            building.buildingObject[destination[i]].transform.parent =
                SelectionOutlineController.instance.target.transform;

            destinationSpriteObject[i] = new GameObject("DestinationSprite");
            destinationSpriteObject[i].transform.position = destinationPosition;
            destinationSpriteObject[i].transform.rotation = SpriteRotation;
            destinationSpriteObject[i].transform.localScale = ObjectScale;

            pizzaSpriteRenderer[i] = destinationSpriteObject[i].AddComponent<SpriteRenderer>();
            pizzaSpriteRenderer[i].sprite = destinationSprite;
            destinationSpriteObject[i].transform.SetParent(parent);

            destinationSpriteObject[i].layer = 8;
        }
        SelectionOutlineController.instance.ClearTarget();
    }
}