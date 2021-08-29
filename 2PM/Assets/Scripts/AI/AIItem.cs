﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIItem : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    AIRBController rbScript;
    
    private int?[] myItems;
    int itemCnt;
    bool isItemCol;

    public bool isSlow;
    private float slowTimer;
    public float slowTime;
    public float orMaxSpeed;

    CreateAIID ID;

    void Start()
    {
        myItems = new int?[2] { null, null };
        itemCnt = 0;
        isItemCol = false;

        //itemType = new int[4];

        ID = transform.Find("AIID").GetComponent<CreateAIID>();
        gameClient.OnUseItem += UseItemToPlayer;
    }

    void Update()
    {
        if (isItemCol)
        {
            RandomItem();
            itemCnt++;
            isItemCol = false;
        }

        if (itemCnt > 0)
            UseItem();

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

    private void RandomItem()
    {
        int index = Random.Range(0, 4);
        //MyItems배열 체크
        switch (itemCnt)        //보유 아이템 개수
        {
            case 0:
                myItems[0] = index;
                break;
            case 1:
                myItems[1] = index;
                break;
            default:
                break;
        }
    }

    void UseItem()
    {
        if (gameClient.client_host && myItems[0] != -1)
        {
            gameClient.UseItem(myItems[0].Value, Random.Range(1, 5), false, 0);
            if (myItems[1] != -1)
            {
                myItems[0] = myItems[1];
                myItems[1] = -1;
            }
            else
            {
                myItems[0] = -1;
            }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "ItemBox")
        {
            if (itemCnt < 2)
            {
                Destroy(other.gameObject);
                isItemCol = true;
            }
        }
    }

    enum ItemType
    {
        blind,
        cloud,
        can,
        rocket,
    }
}
