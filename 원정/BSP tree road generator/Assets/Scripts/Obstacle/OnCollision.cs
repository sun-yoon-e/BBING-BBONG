using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    Obstacle obstacle;
    private void Start()
    {
        obstacle = GameObject.Find("Obstacle").GetComponent<Obstacle>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            obstacle.GenerateCar(false);

            Destroy(gameObject);
        }
    }
}
