using UnityEngine;

public class Car: MonoBehaviour
{
    ObstacleGenerator obstacle;

    private void Start()
    {
        obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            obstacle.GenerateCar(false);

            Destroy(transform.parent.gameObject);
        }
    }
}
