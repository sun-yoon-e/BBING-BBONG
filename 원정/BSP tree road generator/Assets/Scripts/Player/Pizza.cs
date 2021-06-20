using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    private GameClient gameClient = GameClient.Instance;

    public float speed = 20f;
    public float lifeTime = 10f;

    private float lifeTimer;

    Destination destination;
    PlacementBuilding building;

    void Start()
    {
        destination = GameObject.Find("Destination").GetComponent<Destination>();
        building = GameObject.Find("BuildingGenerator").GetComponent<PlacementBuilding>();

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
        if (col.collider.tag =="buildingBoxCollider"
            || col.collider.tag == "map")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < destination.destinationNum; ++i)
        {
            if (building.buildingObject[destination.destination[i]] == other.gameObject)
            {
                Destroy(destination.destinationSpriteObject[i]);
                Destroy(destination.pizzaSpriteRenderer[i]);

                Destroy(destination.destinationPizzaObject[i]);

                destination.DestroyDestination += 1;
            }
        }

        //if (other.gameObject.name != "Player")
        //{
        //    Destroy(gameObject);

        //    if (gameObject.CompareTag("Player"))
        //    {
        //        gameClient.UpdateScore();
        //    }
        //}
    }
}
