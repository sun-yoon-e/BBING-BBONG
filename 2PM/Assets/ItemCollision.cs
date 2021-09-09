using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollision : MonoBehaviour
{
    ItemBoxGenerator itemObject;

    private void Start()
    {
        itemObject = GameObject.Find("Item Generator").GetComponent<ItemBoxGenerator>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("ItemBox"))
        {
            if (Item.ItemCnt < 2)
            {
                itemObject.DestroyItem(col.gameObject);
                Item.ItemCol = true;
            }
        }
    }
}