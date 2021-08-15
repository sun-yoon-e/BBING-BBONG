using Gadd420;
using UnityEngine;
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
    private int useIndex;

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
    private float orMaxSpeed;
    
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
        MyItems = new int?[2] { null, null };
    }

    private void Update()
    {
        if (ItemCol)
        {
            RandomItem();
            ItemCnt++;
            ChangeSprite();
            ItemCol = false;
        }
        
        //아이템 치트키
        if (Input.GetKeyDown(KeyCode.F1))
        {
            cheat = true;
            cheatSlot = 0;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            cheat = true;
            cheatSlot = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (cheat)
            {
                if (MyItems[cheatSlot] == null)
                {
                    ItemCnt++;
                }
                MyItems[cheatSlot] = 0;
                ChangeSprite();
                cheat = false;
            }
            else
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
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (cheat)
            {
                if (MyItems[cheatSlot] == null)
                {
                    ItemCnt++;
                }
                MyItems[cheatSlot] = 1;
                ChangeSprite();
                cheat = false;
            }
            else
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
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MyItems[cheatSlot] = 2;
            ChangeSprite();
            cheat = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MyItems[cheatSlot] = 3;
            ChangeSprite();
            cheat = false;
        }
        
        if (isSlow)
        {
            slowTimer += Time.deltaTime;
            if (slowTimer >= 20f)
            {
                isSlow = false;
                rbScript.maxSpeed = orMaxSpeed;
                slowTimer = 0f;
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
                rbScript.maxSpeed = orMaxSpeed / 2;
                isSlow = true;
                break;
            case 3:
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