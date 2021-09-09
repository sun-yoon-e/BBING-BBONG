using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIItem : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    NitrousManager nitrousScript;
    AIRBController rbScript;
    
    public static int ItemCnt;
    public static bool ItemCol;
    private int?[] MyItems;
    //0: 한명만 시야차단, 1: 나빼고 다 시야차단, 2: 이속 저하, 3: 부스터

    public bool isSlow;
    private float slowTimer;
    public float slowTime;
    public float orMaxSpeed;

    public int playerID;
    public bool isAI;
    public int AIID;

    CreateAIID ID;

    void Start()
    {
        nitrousScript = GetComponent<NitrousManager>();
        rbScript = GetComponent<AIRBController>();
        
        orMaxSpeed = rbScript.maxSpeed;
        ItemCnt = 0;
        MyItems = new int?[2] {-1, -1};

        ID = transform.Find("AIID").GetComponent<CreateAIID>();
        gameClient.OnUseItem += UseItemToPlayer;
    }

    void Update()
    {
        if (ItemCol)
        {
            RandomItem();
            CheckItemCnt();
            ItemCol = false;
            //Debug.Log("AI" + AIID + ":" + MyItems[0] + MyItems[1]);
        }

        if (ItemCnt > 0)
        {
            UseItem(MyItems[0].Value);
            CheckItemCnt();
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
        int index = Random.Range(0, 4);
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

    public void UseItemToPlayer(object sender, ItemMessageEventArgs args)
    {
        if (gameClient.client_host && args.isAI)
        {
            if (args.AIID == ID.idNum)
            {
                SoundManager.instance.PlaySE("Slow_Item");
                rbScript.maxSpeed = orMaxSpeed / 2;
                isSlow = true;
            }
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

    void UseItem(int itemIndex)
    {
        if (gameClient.client_host && MyItems[0] != -1)
        {
            //playerID = Random.Range(1, 5);
            playerID = FindNearestPlayer();

            if (gameClient.ai_client[playerID - 1])
            {
                AIID = playerID - 1;
                isAI = true;
                playerID = 1;
            }
            else
            {
                isAI = false;
                AIID = 0;
            }

            switch (itemIndex)
            {
                case 0:         //모두 시야차단
                    //Debug.Log("AI" + AIID + ": 모두 시야차단 사용");
                    gameClient.UseItem(0, 0, isAI, AIID);
                    break;
                case 1:         //한 명 시야차단
                    //Debug.Log("AI" + AIID + ": 한명 시야차단 사용");
                    gameClient.UseItem(1, playerID, isAI, AIID);
                    break;
                case 2:         //슬로우
                    //Debug.Log("AI" + AIID + ": 슬로우 사용");
                    gameClient.UseItem(2, playerID, isAI, AIID);
                    break;
                case 3:         //부스터
                    //Debug.Log("AI" + AIID + ": 부스터 사용");
                    break;
            }

            if (MyItems[1] != -1)
            {
                MyItems[0] = MyItems[1];
                MyItems[1] = -1;
            }
            else
            {
                MyItems[0] = -1;
            }
            //Debug.Log("AI" + AIID + ":" + MyItems[0] + MyItems[1]);
        }
    }
}
