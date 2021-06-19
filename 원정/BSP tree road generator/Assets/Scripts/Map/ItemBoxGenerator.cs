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
    public GameObject[] itemSpriteObject;
    public SpriteRenderer[] itemSpriteRenderer;
    
    int[] itemBoxPlace;

    RoadGenerator road;
    bool[] roadPlace;

    void Start()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        roadPlace = road.isRoad;

        itemBoxPlace = new int[itemBoxNum];
        itemSpriteObject = new GameObject[itemBoxNum];
        itemSpriteRenderer = new SpriteRenderer[itemBoxNum];
        
        DrawItemBox();
    }

    public void DrawItemBox()
    {
        Quaternion SpriteRotation = Quaternion.Euler(90, 0, 0);
        for (int i = 0; i < itemBoxNum; ++i)
        {
            itemBoxPlace[i] = Random.Range(1, road.roadPositionNum);
            //Vector3 ObjectScale = new Vector3(5, 5, 5);

            //중복체크
            for (int j = 0; j < itemBoxNum; ++j)
            {
                if (itemBoxPlace[i] == itemBoxPlace[j])
                    itemBoxPlace[i] = Random.Range(1, road.roadPositionNum);
            }

            Vector3 itemPosition =
                new Vector3(road.roadPosition[itemBoxPlace[i]].x,
                road.roadPosition[itemBoxPlace[i]].y + 1,
                road.roadPosition[itemBoxPlace[i]].z);

            GameObject Item = Instantiate(itemBoxPrefab, itemPosition, Quaternion.identity, parent);
            Item.tag = "ItemBox";
            Item.transform.localScale = new Vector3(itemScale, itemScale, itemScale);
            
            itemSpriteObject[i] = new GameObject("DestinationSprite");
            itemSpriteObject[i].transform.position = itemPosition;
            itemSpriteObject[i].transform.rotation = SpriteRotation;
            //itemSpriteObject[i].transform.localScale = ObjectScale;

            itemSpriteRenderer[i] = itemSpriteObject[i].AddComponent<SpriteRenderer>();
            itemSpriteRenderer[i].sprite = itemSprite;
            itemSpriteObject[i].transform.SetParent(parent);

            itemSpriteObject[i].layer = 8;
        }
    }
}
