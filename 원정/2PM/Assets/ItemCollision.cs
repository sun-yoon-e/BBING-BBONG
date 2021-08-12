using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("ItemBox"))
        {
            if (Item.ItemCnt < 2)
            {
                Destroy(col.gameObject);
                Item.ItemCol = true;
            }
        }
    }
}
