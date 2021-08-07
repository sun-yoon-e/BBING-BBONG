using UnityEngine;
using UnityEngine.UI;
//using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[5];
    public Image[] images = new Image[2];
    public Sprite nullSprite;

    private int ItemCnt;
    private int?[] MyItems;
    //0: 한명만 시야차단, 1: 나빼고 다 시야차단, 2: 이속 저하, 3: 부스터, 4: 보호막
    private int useIndex;

    GameObject player;
    GameObject miniCamera;
    GameObject mainCamera;
    [SerializeField] GameObject fogParticle;
    
    public float range = 100f;
    public Camera cam;
    
    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        player = GameObject.Find("Player");

        miniCamera = GameObject.Find("Minimap Camera");

        ItemCnt = 0;
        MyItems = new int?[2] { null, null };
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (MyItems[0] != null)
            {
                useIndex = 0;
                UseItem(MyItems[0].Value);
                
                if (MyItems[1] != null)
                {
                    MyItems[0] = MyItems[1];
                    MyItems[1] = null;
                }
                else
                {
                    MyItems[0] = null;
                }
                
                ChangeSprite();
                ItemCnt--;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (MyItems[1] != null)
            {
                useIndex = 1;
                UseItem(MyItems[1].Value);
                MyItems[1] = null;
                ChangeSprite();
                ItemCnt--;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("ItemBox"))
        {
            if (ItemCnt < 2)
            {
                RandomItem();
                ItemCnt++;
                ChangeSprite();

                Destroy(col.gameObject);
            }
        }
    }

    private void RandomItem()
    {
        int index = Random.Range(0, sprites.Length);
        //MyItems배열 체크
        switch (ItemCnt)        //보유 아이템 개수
        {
            case 0:
                MyItems[0] = index;
                break;
            case 1:
                MyItems[1] = index;
                break;
            //case 2:
            //    MyItems[0] = MyItems[1];
            //    MyItems[1] = index;
            //    break;
            default:
                break;
        }
    }

    private void ChangeSprite()
    {
        for (int i = 0; i < ItemCnt; ++i)
        {
            if (MyItems[i] != null)
            {
                Sprite select = sprites[MyItems[i].Value];
                images[i].sprite = select;
            }
            else
            {
                images[i].sprite = nullSprite;
            }
        }
    }

    void UseItem(int itemIndex)
    {
        switch (itemIndex)
        {
            case 0:
                Fog();
                break;
            case 1:
                Fog();
                break;
            case 2:
                Debug.Log("슬로우");
                PlayerController.slowdown = true;
                break;
            case 3:
                Debug.Log("부스터");
                PlayerController.booster = true;
                break;
            case 4:
                Debug.Log("쉴드");
                break;
        }
    }
    
    void Fog()
    {
        Vector3 fogPosition = new Vector3(player.transform.position.x,
            player.transform.position.y + 4,
            player.transform.position.z);

        GameObject fog = Instantiate(fogParticle, fogPosition, player.transform.rotation, player.transform);
        Destroy(fog, 20f);


        Vector3 miniFogPosition = new Vector3(miniCamera.transform.position.x,
            miniCamera.transform.position.y - 14,
            miniCamera.transform.position.z);

        GameObject miniFog = Instantiate(fogParticle, miniFogPosition, miniCamera.transform.rotation, miniCamera.transform);
        miniFog.layer = 18;
        Destroy(miniFog, 20f);
    }
}
