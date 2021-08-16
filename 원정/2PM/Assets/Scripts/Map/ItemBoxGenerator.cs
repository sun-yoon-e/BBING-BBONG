using UnityEngine;

public class ItemBoxGenerator : MonoBehaviour
{
    public int itemBoxNum;
    public GameObject itemBoxPrefab;

    public Sprite itemSprite;
    public GameObject[] itemBox;
    public GameObject[] itemSpriteObject;
    public SpriteRenderer[] itemSpriteRenderer;

    int[] itemBoxPlace;

    public float itemScale;

    RoadGenerator road;
    Quaternion rot;

    void Start()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        itemBox = new GameObject[itemBoxNum];
        itemSpriteObject = new GameObject[itemBoxNum];
        itemSpriteRenderer = new SpriteRenderer[itemBoxNum];

        itemBoxPlace = new int[itemBoxNum];

        GenerateItemBox();
    }

    private void Update()
    {
        //rot = GameObject.Find("Player").transform.rotation;

        //foreach (var i in itemSpriteObject)
        //{
        //    if (i == false)
        //        continue;

        //    rot = Quaternion.Euler(90, rot.eulerAngles.y, rot.eulerAngles.z);
        //    i.transform.rotation = rot;
        //}
    }

    public void GenerateItemBox()
    {
        bool isDuplicate;
        for (int i = 0; i < itemBoxNum; ++i)
        {
            itemBoxPlace[i] = Random.Range(1, road.middleRoadNum);

            // 중복체크
            isDuplicate = false;
            for (int j = 0; j < i; ++j)
            {
                if (itemBoxPlace[j] == itemBoxPlace[i])
                    isDuplicate = true;
            }
            if (isDuplicate)
                continue;

            Vector3 itemPosition =
                new Vector3(road.passibleItemPlace[itemBoxPlace[i]].x,
                road.passibleItemPlace[itemBoxPlace[i]].y + 1.5f,
                road.passibleItemPlace[itemBoxPlace[i]].z);

            itemBox[i] = Instantiate(itemBoxPrefab, itemPosition, Quaternion.identity, transform);
            itemBox[i].transform.localScale = new Vector3(itemScale, itemScale, itemScale);
            itemBox[i].tag = "ItemBox";

            itemSpriteObject[i] = new GameObject("ItemSprite");
            itemSpriteObject[i].transform.position = new Vector3(itemPosition.x, itemPosition.y + 50, itemPosition.z);
            itemSpriteObject[i].transform.rotation = Quaternion.Euler(90, 0, 0);
            itemSpriteObject[i].transform.localScale = new Vector3(2, 2, 2);
            itemSpriteObject[i].transform.SetParent(itemBox[i].transform);
            itemSpriteObject[i].layer = 8;

            itemSpriteRenderer[i] = itemSpriteObject[i].AddComponent<SpriteRenderer>();
            itemSpriteRenderer[i].sprite = itemSprite;
        }
    }
}