using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 10f;

    private float lifeTimer;

    Destination destination;
    PlacementBuilding building;

    void Start()
    {
        destination = GameObject.Find("Destination Generator").GetComponent<Destination>();
        building = GameObject.Find("Building Generator").GetComponent<PlacementBuilding>();

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
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Door")
        {
            for (int i = 0; i < destination.destinationNum; ++i)
            {
                if (col.transform.parent.gameObject == building.buildingObject[destination.destination[i]])
                {
                    Destroy(destination.destinationSpriteObject[i]);
                    Destroy(destination.pizzaSpriteRenderer[i]);

                    Destroy(destination.destinationObject[i]);

                    destination.DestroyDestination += 1;

                    Destroy(gameObject);
                }
                
            }
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if(col.transform.tag == "buildingBoxCollider")
        {
            Destroy(gameObject);
        }
    }

}