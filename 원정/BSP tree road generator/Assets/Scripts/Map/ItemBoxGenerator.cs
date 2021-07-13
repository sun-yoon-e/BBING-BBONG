using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxGenerator : MonoBehaviour
{
    [SerializeField] private int itemBoxNum;
    [SerializeField] private GameObject itemBoxPrefab;

    [SerializeField] private Transform parent;
    [SerializeField] private float itemScale;

    public Sprite itemSprite;
    public GameObject[] itemBox;
    public GameObject[] itemSpriteObject;
    public SpriteRenderer[] itemSpriteRenderer;
    
    int[] itemBoxPlace;

    RoadGenerator road;
    Quaternion rot;

    void Start()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
       
        itemBoxPlace = new int[itemBoxNum];

        itemBox = new GameObject[itemBoxNum];
        itemSpriteObject = new GameObject[itemBoxNum];
        itemSpriteRenderer = new SpriteRenderer[itemBoxNum];
        
        DrawItemBox();
    }

    private void Update()
    {
        rot = GameObject.Find("Player").transform.rotation;

        foreach (var i in itemSpriteObject)
        {
            if (i == false)
                continue;

            rot = Quaternion.Euler(90, rot.eulerAngles.y, rot.eulerAngles.z);
            i.transform.rotation = rot;
        }
    }

    public void DrawItemBox()
    {
        Quaternion SpriteRotation = Quaternion.Euler(90, 0, 0);
        for (int i = 0; i < itemBoxNum; ++i)
        {
            itemBoxPlace[i] = Random.Range(1, road.roadPositionNum);
            Vector3 ObjectScale = new Vector3(2, 2, 2);

            //중복체크
            for (int j = 0; j < itemBoxNum; ++j)
            {
                if (itemBoxPlace[i] == itemBoxPlace[j])
                    itemBoxPlace[i] = Random.Range(1, road.roadPositionNum);
            }

            Vector3 itemPosition =
                new Vector3(road.roadPosition[itemBoxPlace[i]].x,
                road.roadPosition[itemBoxPlace[i]].y + 1.5f,
                road.roadPosition[itemBoxPlace[i]].z);

            itemBox[i] = Instantiate(itemBoxPrefab, itemPosition, Quaternion.identity, parent);
            itemBox[i].transform.localScale = new Vector3(itemScale, itemScale, itemScale);
            itemBox[i].tag = "ItemBox";

            Vector3 spritePosition =
                new Vector3(itemPosition.x, itemPosition.y + 50, itemPosition.z);
            
            itemSpriteObject[i] = new GameObject("DestinationSprite");
            itemSpriteObject[i].transform.position = spritePosition;
            itemSpriteObject[i].transform.rotation = SpriteRotation;
            itemSpriteObject[i].transform.localScale = ObjectScale;

            itemSpriteRenderer[i] = itemSpriteObject[i].AddComponent<SpriteRenderer>();
            itemSpriteRenderer[i].sprite = itemSprite;
            itemSpriteObject[i].transform.SetParent(itemBox[i].transform);

            itemSpriteObject[i].layer = 8;
        }
    }
}
