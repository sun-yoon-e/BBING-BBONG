using Gadd420;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
//using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    NitrousManager nitrousScript;
    RB_Controller rbScript;
    
    public Sprite[] sprites = new Sprite[4];
    public Image[] images = new Image[2];
    public Sprite nullSprite;

    public static int ItemCnt;
    public static bool ItemCol;
    private int?[] MyItems;
    //0: 한명만 시야차단, 1: 나빼고 다 시야차단, 2: 이속 저하, 3: 부스터

    GameObject player;
    GameObject miniCamera;
    GameObject fpsCamera;
    GameObject tpsCamera;
    [SerializeField] GameObject fogParticle;
    
    public float range = 100f;
    public Camera cam;

    public bool cheat;
    public int cheatSlot;
    
    public bool isSlow;
    private float slowTimer;
    public float slowTime;
    public float orMaxSpeed;
    
    private void Start()
    {
        fpsCamera = GameObject.Find("FPS Camera");
        tpsCamera = GameObject.Find("TPS Camera");
        player = GameObject.Find("Player");
        miniCamera = GameObject.Find("Minimap Camera");
        nitrousScript = GetComponent<NitrousManager>();
        rbScript = GetComponent<RB_Controller>();

        orMaxSpeed = rbScript.maxSpeed;
        ItemCnt = 0;
        MyItems = new int?[2] { -1, -1 };
    }

    private void Update()
    {
        if (ItemCol)
        {
            RandomItem();
            CheckItemCnt();
            ChangeSprite();
            ItemCol = false;
            //GameClient.Instance.RemoveItemBox();
        }

        //아이템 치트키
        if (Input.GetKeyDown(KeyCode.F1))
        {
            cheat = true;
            cheatSlot = 0;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (MyItems[0] != -1)
            {
                cheat = true;
                cheatSlot = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (cheat)
            {
                MyItems[cheatSlot] = 0;
                CheckItemCnt();
                ChangeSprite();
                cheat = false;
            }
            else
            {
                if (MyItems[0] != -1)
                {
                    UseItem(MyItems[0].Value);
                
                    if (MyItems[1] != -1)
                    {
                        MyItems[0] = MyItems[1];
                        MyItems[1] = -1;
                    }
                    else
                    {
                        MyItems[0] = -1;
                    }

                    CheckItemCnt();
                    ChangeSprite();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (cheat)
            {
                MyItems[cheatSlot] = 1;
                CheckItemCnt();
                ChangeSprite();
                cheat = false;
            }
            else
            {
                if (MyItems[1] != -1)
                {
                    UseItem(MyItems[1].Value);
                    MyItems[1] = -1;
                    CheckItemCnt();
                    ChangeSprite();
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (cheat)
            {
                MyItems[cheatSlot] = 2;
                CheckItemCnt();
                ChangeSprite();
                cheat = false;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (cheat)
            {
                MyItems[cheatSlot] = 3;
                CheckItemCnt();
                ChangeSprite();
                cheat = false;
            }
        }
        
        if (isSlow)
        {
            slowTimer += Time.deltaTime;
            if (slowTimer >= slowTime)
            {
                isSlow = false;
                rbScript.maxSpeed = orMaxSpeed;
                slowTimer = 0f;
            }
        }
    }

    void CheckItemCnt()
    {
        ItemCnt = 0;
        for (int i = 0; i < MyItems.Length; i++)
        {
            if (MyItems[i] != -1)
            {
                ItemCnt++;
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
            default:
                break;
        }
    }

    private void ChangeSprite()
    {
        for (int i = 0; i < MyItems.Length; ++i)
        {
            if (MyItems[i] != -1)
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
            case 0:         //시야차단
                SoundManager.instance.PlaySE("Smoke_Item");
                Fog();
                break;
            case 1:         //시야차단
                SoundManager.instance.PlaySE("Smoke_Item");
                Fog();
                break;
            case 2:         //슬로우
                SoundManager.instance.PlaySE("Slow_Item");
                rbScript.maxSpeed = orMaxSpeed / 2;
                isSlow = true;
                break;
            case 3:         //부스터
                SoundManager.instance.PlaySE("Boost_Item");
                nitrousScript.isBoosting = true;
                break;
        }
    }

    void Fog()
    {
        //Vector3 fogPosition = new Vector3(0, 0, 0);

        GameObject fog;
        if (fpsCamera.activeSelf)
        {
            fog = Instantiate(fogParticle, fpsCamera.transform.position, fpsCamera.transform.rotation, fpsCamera.transform);

            Destroy(fog, 20f);
        }
        else if (tpsCamera.activeSelf)
        {

            fog = Instantiate(fogParticle, tpsCamera.transform.position, tpsCamera.transform.rotation, tpsCamera.transform);

            Destroy(fog, 20f);
        }

        Vector3 miniFogPosition = new Vector3(miniCamera.transform.position.x,
            miniCamera.transform.position.y - 14,
            miniCamera.transform.position.z);

        GameObject miniFog = Instantiate(fogParticle, miniFogPosition, miniCamera.transform.rotation, miniCamera.transform);
        miniFog.layer = 18;
        Destroy(miniFog, 20f);
    }
}