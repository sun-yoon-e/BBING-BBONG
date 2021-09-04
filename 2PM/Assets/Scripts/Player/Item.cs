using System.Linq;
using Gadd420;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
//using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    NitrousManager nitrousScript;
    RB_Controller rbScript;

    public Sprite[] sprites = new Sprite[4];
    public Image[] images = new Image[2];
    public Sprite nullSprite;

    public static int ItemCnt;
    public static bool ItemCol;

    private int?[] MyItems;
    //0: 한명만 시야차단, 1: 나빼고 다 시야차단, 2: 이속 저하, 3: 부스터

    GameObject miniCamera;
    GameObject fpsCamera;
    GameObject tpsCamera;
    [SerializeField] GameObject fogParticle;

    public bool cheat;
    public int cheatSlot;

    public bool isSlow;
    private float slowTimer;
    public float slowTime;
    public float orMaxSpeed;

    public int targetID;
    public bool isAI;
    public int AIID;

    GameObject fog;

    private void Start()
    {
        fpsCamera = GameObject.Find("FPS Camera");
        tpsCamera = GameObject.Find("TPS Camera");
        miniCamera = GameObject.Find("Minimap Camera");
        nitrousScript = GetComponent<NitrousManager>();
        rbScript = GetComponent<RB_Controller>();

        orMaxSpeed = rbScript.maxSpeed;
        ItemCnt = 0;
        MyItems = new int?[2] { -1, -1 };

        targetID = 0;
        isAI = false;
        AIID = 0;

        gameClient.OnUseItem += UseItemToPlayer;
    }

    private void Update()
    {
        if (ItemCol)
        {
            Debug.Log("아이템 충돌");
            RandomItem();
            CheckItemCnt();
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

    private void LateUpdate()
    {
        if (fog != null)
        {
            if (PlayerCamera.instance.nowCam == 1)
            {
                print("fpsCamera position" + fpsCamera.transform.position);
                print("fpsCamera rotation" + fpsCamera.transform.rotation);

                fog.transform.position = fpsCamera.transform.position;
                fog.transform.rotation = fpsCamera.transform.rotation;
            }
            else if (PlayerCamera.instance.nowCam == 3)
            {
                fog.transform.position = tpsCamera.transform.position;
                fog.transform.rotation = tpsCamera.transform.rotation;
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
        switch (ItemCnt) //보유 아이템 개수
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

    public void UseItemToPlayer(object sender, ItemMessageEventArgs args)
    {
        if (!args.isAI)
        {
            // 대기실 순번대로 숫자에 해당하는 유저에게만 패킷 돌아옴
            // 여기 들어왔다는 것은 내가 아이템 타겟으로 지목당했다는 것
            switch (args.ItemType)
            {
                case 0: //모두 시야차단
                    SoundManager.instance.PlaySE("Smoke_Item");
                    Fog();
                    break;
                case 1: //한 명 시야차단
                    SoundManager.instance.PlaySE("Smoke_Item");
                    Fog();
                    break;
                case 2: //슬로우
                    SoundManager.instance.PlaySE("Slow_Item");
                    rbScript.maxSpeed = orMaxSpeed / 2;
                    isSlow = true;
                    break;
                case 3: //부스터
                    // 얘는 밑에~
                    break;
            }
        }
    }

    void UseItem(int itemIndex)
    {
        // targetID는 1~4로 랜덤설정(?)
        // 1~4는 대기실 순번임
        // 나의 대기실 번호는 gameClient.playerRoomNum(0~3)
        // gameClient.ai_client[targetID - 1]가 true이면 AI 플레이어
        // 비교해서 AI이면 isAI를 true로, targetID - 1를 AIID로 넣어주면 됨

        //targetID = Random.Range(1, 5);
        targetID = FindNearestPlayer();

        if (gameClient.ai_client[targetID - 1])
        {
            AIID = targetID - 1;
            isAI = true;
            targetID = 1;
        }
        else
        {
            isAI = false;
            AIID = 0;
        }

        switch (itemIndex)
        {
            case 0: //모두 시야차단
                gameClient.UseItem(0, 0, isAI, AIID);
                break;
            case 1: //한 명 시야차단
                gameClient.UseItem(1, targetID, isAI, AIID);
                break;
            case 2: //슬로우
                gameClient.UseItem(2, targetID, isAI, AIID);
                break;
            case 3: //부스터
                SoundManager.instance.PlaySE("Boost_Item");
                nitrousScript.isBoosting = true;
                break;
        }
    }

    int FindNearestPlayer()
    {
        int index = 0;
        int target = 0;
        float dis = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject player in GameSceneMain.instacne.players)
        {
            if (player != null)
            {
                index++;
                if (index != gameClient.playerRoomNum + 1)
                {
                    Vector3 diff = player.transform.position - position;
                    float curDis = diff.sqrMagnitude;
                    if (curDis < dis)
                    {
                        target = index;
                        dis = curDis;
                    }
                }
            }
        }

        return target;
    }

    void Fog()
    {
        //Vector3 fogPosition = new Vector3(0, 0, 0);

        fog = Instantiate(fogParticle, transform.position, transform.rotation, transform);

        Destroy(fog, 20f);
        //if (fpsCamera.activeSelf)
        //{
        //    fog = Instantiate(fogParticle, fpsCamera.transform.position, fpsCamera.transform.rotation, fpsCamera.transform);

        //    Destroy(fog, 20f);
        //}
        //else if (tpsCamera.activeSelf)
        //{

        //    fog = Instantiate(fogParticle, tpsCamera.transform.position, tpsCamera.transform.rotation, tpsCamera.transform);

        //    Destroy(fog, 20f);
        //}
        //else
        //{
        //    fog = Instantiate(fogParticle, tpsCamera.transform.position, tpsCamera.transform.rotation, tpsCamera.transform);

        //    Destroy(fog, 20f);
        //}

        Vector3 miniFogPosition = new Vector3(miniCamera.transform.position.x,
            miniCamera.transform.position.y - 14,
            miniCamera.transform.position.z);

        GameObject miniFog = Instantiate(fogParticle, miniFogPosition, miniCamera.transform.rotation, miniCamera.transform);
        miniFog.layer = 18;
        Destroy(miniFog, 20f);
    }
}