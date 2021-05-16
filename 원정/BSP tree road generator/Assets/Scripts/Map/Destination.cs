using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    PlacementBuilding building;

    public GameObject destinationPrefab;
    public Sprite destinationSprite;
    public Transform parent;

    public int[] destination;
    public int destinationNum;

    bool[] isDestination;
    
    private void Start()
    {
        building = GameObject.Find("BuildingGenerator").GetComponent<PlacementBuilding>();
        destination = new int[destinationNum];
        isDestination = new bool[building.buildingNum];

        DrawDestination();
        ApplyDestinationToBuilding();
    }

    public void DrawDestination()
    {
        for (int i = 0; i < destinationNum; ++i)
        {
            destination[i] = Random.Range(1, building.buildingNum);
            isDestination[destination[i]] = true;
            
            //중복체크
            for (int j = 0; j < destinationNum; ++j)
            {
                if(destination[i] == destination[j])
                    destination[i] = Random.Range(1, building.buildingNum);
            }
        }
    }

    void ApplyDestinationToBuilding()
    {
        Vector3 spriteScale = new Vector3(10, 10, 10);

        Quaternion SpriteRotation = Quaternion.Euler(90, 0, 0);
        Vector3 ObjectScale = new Vector3(10, 10, 10);

        for (int i = 0; i < destinationNum; ++i)
        {
            Vector3 destinationPosition = 
                new Vector3(building.buildingObject[destination[i]].transform.position.x,
                building.buildingObject[destination[i]].transform.position.y + 20,
                building.buildingObject[destination[i]].transform.position.z);

            Instantiate(destinationPrefab, destinationPosition,
                Quaternion.Euler(0, 0, 0), parent);


            GameObject spriteObject = new GameObject("DestinationSprite");
            spriteObject.transform.position = destinationPosition;
            spriteObject.transform.rotation = SpriteRotation;
            spriteObject.transform.localScale = ObjectScale;

            SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sprite = destinationSprite;
            spriteObject.transform.SetParent(parent);

            spriteObject.layer = 8;
        }
    }
}
