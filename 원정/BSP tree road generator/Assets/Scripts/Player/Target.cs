using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("충돌?");
        if (col.collider.tag == "PizzaBox")
        {
            Debug.Log("충돌");
            Destroy(gameObject);
            Destroy(col.gameObject);
        }
    }
}
