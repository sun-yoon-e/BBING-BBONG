using UnityEngine;
using UnityEngine.UI;
//using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[5];
    public Image[] images = new Image[2];
    public Sprite nullSprite;

    private int ItemCnt = 0;
    private int?[] MyItems = new int?[2];
    //0: 한명만 시야차단, 1: 나빼고 다 시야차단, 2: 이속 저하, 3: 부스터, 4: 보호막

    GameObject player;
    GameObject miniCamera;
    [SerializeField] GameObject fogParticle;
    
    public float range = 100f;
    public Camera cam;
    public static bool Using = false; //아이템 사용중
    private int useIndex;


    private void Start()
    {
        //mainCamera = GameObject.Find("Main Camera");
        player = GameObject.Find("Player");

        miniCamera = GameObject.Find("Mini Camera");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!Using) Using = true;
            if (MyItems[0] != null)
            {
                useIndex = 0;
                UseItem(MyItems[0].Value);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!Using) Using = true;
            if (MyItems[1] != null)
            {
                useIndex = 1;
                UseItem(MyItems[1].Value);
            }
        }
        
        if (Input.GetButton("Fire1") && Using)
        {
            UseItem(MyItems[useIndex].Value); 
            //MyItems[useIndex] = nullSprite;
            Using = false;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("ItemBox"))
        {
            Destroy(col.gameObject);
            RandomItem();
            ChangeSprite();
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
                ItemCnt++;
                break;

            case 1:
                MyItems[1] = index;
                ItemCnt++;
                break;
            case 2:
                MyItems[0] = MyItems[1];
                MyItems[1] = index;
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
        }
    }

    void UseItem(int itemIndex)
    {
        switch (itemIndex)
        {
            case 0:
                Fog();
                //images[itemIndex].sprite = ;
                break;
            case 1:
                Fog();
                images[itemIndex].sprite = null;
                break;
            case 2:
                Debug.Log("슬로우");
                PlayerController.slowdown = true;
                images[itemIndex].sprite = null;
                break;
            case 3:
                Debug.Log("부스터");
                PlayerController.booster = true;
                images[itemIndex].sprite = null;
                break;
            case 4:
                Debug.Log("쉴드");
                images[itemIndex].sprite = null;
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
            miniCamera.transform.position.y - 10,
            miniCamera.transform.position.z);

        GameObject miniFog = Instantiate(fogParticle, miniFogPosition, miniCamera.transform.rotation, miniCamera.transform);
        Destroy(miniFog, 20f);
    }


    //StartCoroutine(LateCall());
    //IEnumerator LateCall()
    //{

    //    yield return new WaitForSeconds(20);

    //    miniMap.SetActive(true);
    //    //Do Function here...
    //}

}
