using UnityEngine;

public class ItemBoxGenerator : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

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

    private void Awake()
    {
        road = GameObject.Find("Road Generator").GetComponent<RoadGenerator>();

        itemBox = new GameObject[itemBoxNum];
        itemSpriteObject = new GameObject[itemBoxNum];
        itemSpriteRenderer = new SpriteRenderer[itemBoxNum];

        itemBoxPlace = new int[itemBoxNum];

        gameClient.OnPlaceItemBox += PlaceItem;
        gameClient.OnRemoveItemBox += RemoveItem;

        road.OnRoadReady3 += CreateItemBox;
        if (road.isRoadReady)
        {
            CreateItemBox(this, System.EventArgs.Empty);
        }

        //GenerateItemBox();
    }

    void CreateItemBox(object sender, System.EventArgs args)
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

            gameClient.PlaceItemBox(i, itemPosition);
            //Debug.Log(itemPosition);
            //Debug.Log("OnRoadItem");
        }
    }

    public void PlaceItem(object sender, PlaceItemBoxMessageEventArgs args)
    {
        itemBox[args.ItemID] = Instantiate(itemBoxPrefab, args.Position, Quaternion.identity, transform);
        itemBox[args.ItemID].transform.localScale = new Vector3(itemScale, itemScale, itemScale);
        itemBox[args.ItemID].tag = "ItemBox";

        itemSpriteObject[args.ItemID] = new GameObject("ItemSprite");
        itemSpriteObject[args.ItemID].transform.position = new Vector3(args.Position.x, args.Position.y + 50, args.Position.z);
        itemSpriteObject[args.ItemID].transform.rotation = Quaternion.Euler(90, 0, 0);
        itemSpriteObject[args.ItemID].transform.localScale = new Vector3(2, 2, 2);
        itemSpriteObject[args.ItemID].transform.SetParent(itemBox[args.ItemID].transform);
        itemSpriteObject[args.ItemID].layer = 8;

        itemSpriteRenderer[args.ItemID] = itemSpriteObject[args.ItemID].AddComponent<SpriteRenderer>();
        itemSpriteRenderer[args.ItemID].sprite = itemSprite;
        //Debug.Log("PlaceItem");
    }

    public void RemoveItem(object sender, RemoveItemBoxMessageEventArgs args)
    {
        Debug.Log(args.ItemID);
        Destroy(itemBox[args.ItemID]);
        Destroy(itemSpriteObject[args.ItemID]);
        Destroy(itemSpriteRenderer[args.ItemID]);
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

            gameClient.PlaceItemBox(i, itemBox[i].transform.position);
        }
    }
}