using UnityEngine;

public class OnCollision : MonoBehaviour
{
    Obstacle obstacle;
    private void Start()
    {
        obstacle = GameObject.Find("Obstacle").GetComponent<Obstacle>();
    }

    private void Update()
    {
        //transform.rotation = Quaternion.Euler(0, 180, 0);
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
