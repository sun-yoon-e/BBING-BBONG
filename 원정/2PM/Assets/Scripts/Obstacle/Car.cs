using UnityEngine;
using UnityEngine.AI;

public class Car: MonoBehaviour
{
    ObstacleGenerator obstacle;
    Rigidbody rb;

    private void Start()
    {
        obstacle = GameObject.Find("Obstacle Generator").GetComponent<ObstacleGenerator>();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.localPosition = new Vector3(0, 0, 0);

        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" /*|| collision.gameObject.tag == "mapBoxCollider"*/ )
        {
            obstacle.GenerateCar(false);

            Destroy(transform.parent.gameObject);
        }
    }
}
