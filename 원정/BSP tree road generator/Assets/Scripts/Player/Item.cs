using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[5];
    public Image[] images = new Image[2];

    private int ItemCnt = 0;
    private int[] MyItems = new int[2];
    
    private void OnCollisionEnter(Collision col)
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
                Sprite select = sprites[MyItems[i]];
                images[i].sprite = select;
            }
        }
    }
}
