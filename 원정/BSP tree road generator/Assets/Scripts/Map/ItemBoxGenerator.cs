using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxGenerator : MonoBehaviour
{
    [SerializeField] private int itemBoxNum;
    [SerializeField] private GameObject itemBoxPrefab;

    [SerializeField] private Transform parent;
    [SerializeField] private float itemScale;

    int[] itemBoxPlace;

    RoadGenerator road;
    bool[] roadPlace;

    void Start()
    {
        road = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        roadPlace = road.isRoad;

        itemBoxPlace = new int[itemBoxNum];

        DrawItemBox();
    }

    public void DrawItemBox()
    {
        for (int i = 0; i < itemBoxNum; ++i)
        {
            itemBoxPlace[i] = Random.Range(1, road.roadPositionNum);

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
            Item.transform.localScale = new Vector3(itemScale, itemScale, itemScale);
        }
    }
}
