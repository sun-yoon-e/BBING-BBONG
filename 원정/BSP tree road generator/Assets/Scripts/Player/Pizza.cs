using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 10f;

    private float lifeTimer;
    
    void Start()
    {
        lifeTimer = lifeTime;
    }
    
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Door"
            || col.collider.tag == "buildingBoxCollider"
            || col.collider.tag == "map")
        {
            Destroy(gameObject);
        }
    }
}
