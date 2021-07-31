using UnityEngine;

public class Car: MonoBehaviour
{
    Obstacle obstacle;

    private void Start()
    {
        obstacle = GameObject.Find("Obstacle").GetComponent<Obstacle>();
    }

    private void Update()
    {
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
